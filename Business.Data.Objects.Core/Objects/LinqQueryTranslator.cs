using Business.Data.Objects.Common.Utils;
using Business.Data.Objects.Core.Base;
using Business.Data.Objects.Core.ObjFactory;
using Business.Data.Objects.Core.Schema.Definition;
using Business.Data.Objects.Database;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Business.Data.Objects.Core.Objects
{
    public class LinqQueryTranslator<T> : ExpressionVisitor, IDisposable
        where T : DataObject<T>
    {
        private StringBuilder sb;
        private string _orderBy = string.Empty;

        private ClassSchema _schema;
        private BusinessSlot _slot;
        private IDataBase _db;
        private List<DbParameter> _paramList = new List<DbParameter>();
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

            this._db.AddParameters(this._paramList);

            return this.sb.ToString();
        }

        public string TranslateKey<TKey>(Expression<Func<T, TKey>> expression)
        {
            this.sb = new StringBuilder();
            this.Visit(expression);

            if (this._paramList.Count > 0)
                this._db.AddParameters(this._paramList);

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

            if (m.Method.Name == "CompareString") //Solo VB
            {
                return this.Visit(m.Arguments[0]);
            }
            else if (m.Method.Name == nameof(Extensions.In))
            {
                this.sb.Append(@"(");
                this.Visit(m.Arguments[0]);
                this.sb.Append(@" IN (");

                var ee = m.Arguments[1] as NewArrayExpression;

                if (ee == null)
                {
                    var obj = ((Array)this.execExpression(m.Arguments[1])).Cast<object>();

                    ee = Expression.NewArrayInit(obj.First().GetType(), obj.Select(c => Expression.Constant(c)));

                }

                foreach (var item in ee.Expressions)
                {
                    this.Visit(item);
                    this.sb.Append(@",");
                }


                this.sb.Remove(this.sb.Length - 1, 1);
                
                this.sb.Append(@")");
                this.sb.Append(@")");

                return m;
            }
            else if (m.Method.Name == nameof(Extensions.Between))
            {
                this.sb.Append(@"(");
                this.Visit(m.Arguments[0]);
                this.sb.Append(@" BETWEEN ");

                this.Visit(m.Arguments[1]);
                this.sb.Append(@" AND ");
                this.Visit(m.Arguments[2]);
                this.sb.Append(@")");

                return m;
            }
            else if (m.Method.Name == nameof(Extensions.Like))
            {
                this.sb.Append(@"(");
                this.Visit(m.Arguments[0]);
                this.sb.Append(@" LIKE ");
                this.Visit(m.Arguments[1]);
                this.sb.Append(@")");

                return m;
            }
            else if (m.Method.Name == nameof(Extensions.IsNull))
            {
                this.sb.Append(@"(");
                this.Visit(m.Arguments[0]);
                this.sb.Append(@" IS NULL ");
                this.sb.Append(@")");

                return m;
            }
            else if (m.Method.Name == nameof(Extensions.IsNotNull))
            {
                this.sb.Append(@"(");
                this.Visit(m.Arguments[0]);
                this.sb.Append(@" IS NOT NULL ");
                this.sb.Append(@")");

                return m;
            }
            //else if (m.Method.Name == "Take")
            //{
            //    if (this.ParseTakeExpression(m))
            //    {
            //        Expression nextExpression = m.Arguments[0];
            //        return this.Visit(nextExpression);
            //    }
            //}
            //else if (m.Method.Name == "Skip")
            //{
            //    if (this.ParseSkipExpression(m))
            //    {
            //        Expression nextExpression = m.Arguments[0];
            //        return this.Visit(nextExpression);
            //    }
            //}
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

            this.runExpression(m);
            return m;
        }

        public override Expression Visit(Expression node)
        {
            if (node.NodeType == ExpressionType.New)
            {
                this.runExpression(node);
                return node;
            }

            return base.Visit(node);
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
                this._paramList.Add(this._db.CreateParameter($"@pa_{_parIndex}", c.Value, c.Value.GetType()));
                //this._db.AddParameter($"@pa_{_parIndex}", c.Value);

            }

            return c;
        }


        /// <summary>
        /// Esegue espressione e ritorna output
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private object execExpression(Expression m)
        {
            try
            {
                var lnq = Expression.Lambda(m).Compile();

                return lnq.DynamicInvoke();
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Non e' stato possibile risolvere correttamente l'espressione LINQ '{m}'", e);
            }
        }


        /// <summary>
        /// Compila ed esegue uno statement
        /// </summary>
        /// <param name="m"></param>
        private void runExpression(Expression m)
        {
            try
            {
                this._parIndex++;
                sb.Append($"@pa_{_parIndex}");

                this._paramList.Add(this._db.CreateParameter($"@pa_{_parIndex}", this.execExpression(m), m.Type));

                //this._db.AddParameter($"@pa_{_parIndex}", this.execExpression(m));
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Non e' stato possibile risolvere correttamente l'espressione LINQ '{m}'", e);
            }
        }

        protected override Expression VisitMember(MemberExpression m)
        {

            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                sb.Append(this._slot.DbPrefixGetColumn(this._schema.OriginalType, m.Member.Name));
            }
            else
            {
                this.runExpression(m);
            }

            return m;

        }

        protected bool IsNullConstant(Expression exp)
        {
            return (exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null);
        }

        private bool ParseOrderByExpression(MethodCallExpression expression, string order)
        {


            return false;
        }



        public void Dispose()
        {
            sb.Clear();
        }
    }


}
