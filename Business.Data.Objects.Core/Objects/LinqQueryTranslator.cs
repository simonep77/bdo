using Business.Data.Objects.Core.Base;
using Business.Data.Objects.Core.ObjFactory;
using Business.Data.Objects.Core.Schema.Definition;
using Business.Data.Objects.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Business.Data.Objects.Core.Objects
{
    public class LinqQueryTranslator<T> : ExpressionVisitor
        where T: DataObject<T>
    {
        private StringBuilder sb;
        private string _orderBy = string.Empty;
        private int? _skip = null;
        private int? _take = null;
        private string _whereClause = string.Empty;

        private ClassSchema _schema;
        private BusinessSlot _slot;
        private IDataBase _db;
        private int _parIndex;
        

        public LinqQueryTranslator(BusinessSlot slot)
        {
            this._slot = slot;
            this._schema = ProxyAssemblyCache.Instance.GetClassSchema(typeof(T));
            this._db = this._slot.DbGet(this._schema);
        }

        public string Translate(Expression<Func<T, bool>> expression)
        {
            this.sb = new StringBuilder();
            this.Visit(expression);
      
            return this.sb.ToString();
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.Name == "CompareString")
            {
                return this.Visit(m.Arguments[0]);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                this.Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                return m;
            }
            else if (m.Method.Name == "Take")
            {
                if (this.ParseTakeExpression(m))
                {
                    Expression nextExpression = m.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }
            else if (m.Method.Name == "Skip")
            {
                if (this.ParseSkipExpression(m))
                {
                    Expression nextExpression = m.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }
            else if (m.Method.Name == "OrderBy")
            {
                if (this.ParseOrderByExpression(m, "ASC"))
                {
                    Expression nextExpression = m.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }
            else if (m.Method.Name == "OrderByDescending")
            {
                if (this.ParseOrderByExpression(m, "DESC"))
                {
                    Expression nextExpression = m.Arguments[0];
                    return this.Visit(nextExpression);
                }
            }


            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    sb.Append(" NOT ");
                    this.Visit(u.Operand);
                    break;
                case ExpressionType.Convert:
                    this.Visit(u.Operand);
                    break;
                case ExpressionType.ConvertChecked:
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression b)
        {
            sb.Append("(");
            this.Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.And:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.AndAlso:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.Or:
                    sb.Append(" OR ");
                    break;

                case ExpressionType.OrElse:
                    sb.Append(" OR ");
                    break;

                case ExpressionType.Equal:
                    if (IsNullConstant(b.Right))
                    {
                        sb.Append(" IS ");
                    }
                    else
                    {
                        sb.Append(" = ");
                    }
                    break;

                case ExpressionType.NotEqual:
                    if (IsNullConstant(b.Right))
                    {
                        sb.Append(" IS NOT ");
                    }
                    else
                    {
                        sb.Append(" <> ");
                    }
                    break;

                case ExpressionType.LessThan:
                    sb.Append(" < ");
                    break;

                case ExpressionType.LessThanOrEqual:
                    sb.Append(" <= ");
                    break;

                case ExpressionType.GreaterThan:
                    sb.Append(" > ");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" >= ");
                    break;

                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));

            }

            this.Visit(b.Right);
            sb.Append(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;

            if (q == null && c.Value == null)
            {
                sb.Append("NULL");
            }
            else if (q == null)
            {
                _parIndex++;
                sb.Append($"@pa_{_parIndex}");
                this._db.AddParameter($"@pa_{_parIndex}", c.Value);

            }

            return c;
        }



        protected override Expression VisitMember(MemberExpression m)
        {
           
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                    sb.Append(this._slot.DbPrefixGetColumn(this._schema.OriginalType, m.Member.Name));
            }
            else
            {
                var lnq = Expression.Lambda(m).Compile();

                _parIndex++;
                sb.Append($"@pa_{_parIndex}");
                this._db.AddParameter($"@pa_{_parIndex}", lnq.DynamicInvoke());

            }

            return m;
            //throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        protected bool IsNullConstant(Expression exp)
        {
            return (exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null);
        }

        private bool ParseOrderByExpression(MethodCallExpression expression, string order)
        {
            //UnaryExpression unary = (UnaryExpression)expression.Arguments[1];
            //LambdaExpression lambdaExpression = (LambdaExpression)unary.Operand;

            //lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            //MemberExpression body = lambdaExpression.Body as MemberExpression;
            //if (body != null)
            //{
            //    if (string.IsNullOrEmpty(_orderBy))
            //    {
            //        _orderBy = string.Format("{0} {1}", body.Member.Name, order);
            //    }
            //    else
            //    {
            //        _orderBy = string.Format("{0}, {1} {2}", _orderBy, body.Member.Name, order);
            //    }

            //    return true;
            //}

            return false;
        }

        private bool ParseTakeExpression(MethodCallExpression expression)
        {
            ConstantExpression sizeExpression = (ConstantExpression)expression.Arguments[1];

            int size;
            if (int.TryParse(sizeExpression.Value.ToString(), out size))
            {
                _take = size;
                return true;
            }

            return false;
        }

        private bool ParseSkipExpression(MethodCallExpression expression)
        {
            ConstantExpression sizeExpression = (ConstantExpression)expression.Arguments[1];

            int size;
            if (int.TryParse(sizeExpression.Value.ToString(), out size))
            {
                _skip = size;
                return true;
            }

            return false;
        }
    }
}
