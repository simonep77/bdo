
namespace Business.Data.WinFormTest
{
    partial class frmTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTest));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.lINQ2SQLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jSONTOEFROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lINQ2SQLENHANCEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qUERYMAPSPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qUERYMAPPERSBENCHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tESTLOGICALDELETEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.tESTCacheresultConLinqToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tESTCacheResultSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lINQ2SQLToolStripMenuItem,
            this.jSONTOEFROMToolStripMenuItem,
            this.lINQ2SQLENHANCEToolStripMenuItem,
            this.qUERYMAPSPToolStripMenuItem,
            this.qUERYMAPPERSBENCHToolStripMenuItem,
            this.tESTLOGICALDELETEToolStripMenuItem,
            this.tESTCacheresultConLinqToolStripMenuItem,
            this.tESTCacheResultSizeToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(29, 22);
            this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            // 
            // lINQ2SQLToolStripMenuItem
            // 
            this.lINQ2SQLToolStripMenuItem.Name = "lINQ2SQLToolStripMenuItem";
            this.lINQ2SQLToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.lINQ2SQLToolStripMenuItem.Text = "LINQ 2 SQL";
            this.lINQ2SQLToolStripMenuItem.Click += new System.EventHandler(this.lINQ2SQLToolStripMenuItem_Click);
            // 
            // jSONTOEFROMToolStripMenuItem
            // 
            this.jSONTOEFROMToolStripMenuItem.Name = "jSONTOEFROMToolStripMenuItem";
            this.jSONTOEFROMToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.jSONTOEFROMToolStripMenuItem.Text = "JSON TO e FROM";
            this.jSONTOEFROMToolStripMenuItem.Click += new System.EventHandler(this.jSONTOEFROMToolStripMenuItem_Click);
            // 
            // lINQ2SQLENHANCEToolStripMenuItem
            // 
            this.lINQ2SQLENHANCEToolStripMenuItem.Name = "lINQ2SQLENHANCEToolStripMenuItem";
            this.lINQ2SQLENHANCEToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.lINQ2SQLENHANCEToolStripMenuItem.Text = "QUERY MAPPERS";
            this.lINQ2SQLENHANCEToolStripMenuItem.Click += new System.EventHandler(this.lINQ2SQLENHANCEToolStripMenuItem_Click);
            // 
            // qUERYMAPSPToolStripMenuItem
            // 
            this.qUERYMAPSPToolStripMenuItem.Name = "qUERYMAPSPToolStripMenuItem";
            this.qUERYMAPSPToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.qUERYMAPSPToolStripMenuItem.Text = "QUERY MAP SP";
            this.qUERYMAPSPToolStripMenuItem.Click += new System.EventHandler(this.qUERYMAPSPToolStripMenuItem_Click);
            // 
            // qUERYMAPPERSBENCHToolStripMenuItem
            // 
            this.qUERYMAPPERSBENCHToolStripMenuItem.Name = "qUERYMAPPERSBENCHToolStripMenuItem";
            this.qUERYMAPPERSBENCHToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.qUERYMAPPERSBENCHToolStripMenuItem.Text = "QUERY MAPPERS BENCH";
            this.qUERYMAPPERSBENCHToolStripMenuItem.Click += new System.EventHandler(this.qUERYMAPPERSBENCHToolStripMenuItem_Click);
            // 
            // tESTLOGICALDELETEToolStripMenuItem
            // 
            this.tESTLOGICALDELETEToolStripMenuItem.Name = "tESTLOGICALDELETEToolStripMenuItem";
            this.tESTLOGICALDELETEToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.tESTLOGICALDELETEToolStripMenuItem.Text = "TEST LOGICAL DELETE";
            this.tESTLOGICALDELETEToolStripMenuItem.Click += new System.EventHandler(this.tESTLOGICALDELETEToolStripMenuItem_Click);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(12, 28);
            this.txtLog.MaxLength = 0;
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(776, 410);
            this.txtLog.TabIndex = 1;
            // 
            // tESTCacheresultConLinqToolStripMenuItem
            // 
            this.tESTCacheresultConLinqToolStripMenuItem.Name = "tESTCacheresultConLinqToolStripMenuItem";
            this.tESTCacheresultConLinqToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.tESTCacheresultConLinqToolStripMenuItem.Text = "TEST Cacheresult con Linq";
            this.tESTCacheresultConLinqToolStripMenuItem.Click += new System.EventHandler(this.tESTCacheresultConLinqToolStripMenuItem_Click);
            // 
            // tESTCacheResultSizeToolStripMenuItem
            // 
            this.tESTCacheResultSizeToolStripMenuItem.Name = "tESTCacheResultSizeToolStripMenuItem";
            this.tESTCacheResultSizeToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.tESTCacheResultSizeToolStripMenuItem.Text = "TEST CacheResult Size";
            this.tESTCacheResultSizeToolStripMenuItem.Click += new System.EventHandler(this.tESTCacheResultSizeToolStripMenuItem_Click);
            // 
            // frmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.toolStrip1);
            this.Name = "frmTest";
            this.Text = "BDO Test Form";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem lINQ2SQLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jSONTOEFROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lINQ2SQLENHANCEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qUERYMAPSPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qUERYMAPPERSBENCHToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tESTLOGICALDELETEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tESTCacheresultConLinqToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tESTCacheResultSizeToolStripMenuItem;
    }
}