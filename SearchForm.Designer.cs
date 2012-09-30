namespace Poof
{
	partial class SearchForm
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
			this.tbx_Search = new System.Windows.Forms.TextBox();
			this.dgd_Results = new System.Windows.Forms.DataGridView();
			this.dcl_preview = new System.Windows.Forms.DataGridViewImageColumn();
			this.dcl_Address = new System.Windows.Forms.DataGridViewLinkColumn();
			this.dcl_Tags = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.btn_Options = new System.Windows.Forms.Button();
			this.btn_Scan = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dgd_Results)).BeginInit();
			this.SuspendLayout();
			// 
			// tbx_Search
			// 
			this.tbx_Search.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbx_Search.Location = new System.Drawing.Point(12, 12);
			this.tbx_Search.Name = "tbx_Search";
			this.tbx_Search.Size = new System.Drawing.Size(760, 20);
			this.tbx_Search.TabIndex = 0;
			this.tbx_Search.TextChanged += new System.EventHandler(this.tbx_Search_TextChanged);
			// 
			// dgd_Results
			// 
			this.dgd_Results.AllowUserToAddRows = false;
			this.dgd_Results.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dgd_Results.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgd_Results.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dcl_preview,
            this.dcl_Address,
            this.dcl_Tags});
			this.dgd_Results.Location = new System.Drawing.Point(12, 38);
			this.dgd_Results.Name = "dgd_Results";
			this.dgd_Results.Size = new System.Drawing.Size(760, 476);
			this.dgd_Results.TabIndex = 1;
			this.dgd_Results.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_Results_CellClick);
			this.dgd_Results.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_Results_CellDoubleClick);
			this.dgd_Results.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgd_Results_CellEndEdit);
			// 
			// dcl_preview
			// 
			this.dcl_preview.HeaderText = "Preview";
			this.dcl_preview.Name = "dcl_preview";
			this.dcl_preview.ReadOnly = true;
			this.dcl_preview.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.dcl_preview.Width = 200;
			// 
			// dcl_Address
			// 
			this.dcl_Address.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.dcl_Address.DefaultCellStyle = dataGridViewCellStyle1;
			this.dcl_Address.HeaderText = "Address";
			this.dcl_Address.Name = "dcl_Address";
			this.dcl_Address.ReadOnly = true;
			this.dcl_Address.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.dcl_Address.Width = 51;
			// 
			// dcl_Tags
			// 
			this.dcl_Tags.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.dcl_Tags.HeaderText = "Tags";
			this.dcl_Tags.Name = "dcl_Tags";
			this.dcl_Tags.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// btn_Options
			// 
			this.btn_Options.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Options.Location = new System.Drawing.Point(672, 520);
			this.btn_Options.Name = "btn_Options";
			this.btn_Options.Size = new System.Drawing.Size(100, 30);
			this.btn_Options.TabIndex = 3;
			this.btn_Options.Text = "Options";
			this.btn_Options.UseVisualStyleBackColor = true;
			this.btn_Options.Click += new System.EventHandler(this.btn_Options_Click);
			// 
			// btn_Scan
			// 
			this.btn_Scan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btn_Scan.Location = new System.Drawing.Point(12, 520);
			this.btn_Scan.Name = "btn_Scan";
			this.btn_Scan.Size = new System.Drawing.Size(100, 30);
			this.btn_Scan.TabIndex = 4;
			this.btn_Scan.Text = "Rescan";
			this.btn_Scan.UseVisualStyleBackColor = true;
			this.btn_Scan.Click += new System.EventHandler(this.btn_Scan_Click);
			// 
			// SearchForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 562);
			this.Controls.Add(this.btn_Scan);
			this.Controls.Add(this.btn_Options);
			this.Controls.Add(this.dgd_Results);
			this.Controls.Add(this.tbx_Search);
			this.DoubleBuffered = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SearchForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Poof";
			((System.ComponentModel.ISupportInitialize)(this.dgd_Results)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbx_Search;
		private System.Windows.Forms.DataGridView dgd_Results;
		private System.Windows.Forms.Button btn_Options;
		private System.Windows.Forms.DataGridViewImageColumn dcl_preview;
		private System.Windows.Forms.DataGridViewLinkColumn dcl_Address;
		private System.Windows.Forms.DataGridViewTextBoxColumn dcl_Tags;
		private System.Windows.Forms.Button btn_Scan;
	}
}