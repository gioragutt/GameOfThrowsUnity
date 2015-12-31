namespace GotServer
{
    partial class ServerForm
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.clientsListBox = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.messagesTextbox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.txtStatusMessage = new System.Windows.Forms.TextBox();
            this.statusReseterTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(93, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Status:";
            // 
            // clientsListBox
            // 
            this.clientsListBox.FormattingEnabled = true;
            this.clientsListBox.Location = new System.Drawing.Point(6, 19);
            this.clientsListBox.Name = "clientsListBox";
            this.clientsListBox.ScrollAlwaysVisible = true;
            this.clientsListBox.Size = new System.Drawing.Size(369, 147);
            this.clientsListBox.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.clientsListBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 290);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(385, 171);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Client List";
            // 
            // messagesTextbox
            // 
            this.messagesTextbox.BackColor = System.Drawing.SystemColors.Window;
            this.messagesTextbox.Location = new System.Drawing.Point(6, 19);
            this.messagesTextbox.Multiline = true;
            this.messagesTextbox.Name = "messagesTextbox";
            this.messagesTextbox.ReadOnly = true;
            this.messagesTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.messagesTextbox.Size = new System.Drawing.Size(369, 166);
            this.messagesTextbox.TabIndex = 6;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.messagesTextbox);
            this.groupBox2.Location = new System.Drawing.Point(12, 93);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(385, 191);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Messages";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(12, 12);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 8;
            this.btnExit.Text = "Exit";
            this.btnExit.UseMnemonic = false;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // txtStatusMessage
            // 
            this.txtStatusMessage.BackColor = System.Drawing.SystemColors.Window;
            this.txtStatusMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.txtStatusMessage.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtStatusMessage.Location = new System.Drawing.Point(6, 19);
            this.txtStatusMessage.Name = "txtStatusMessage";
            this.txtStatusMessage.ReadOnly = true;
            this.txtStatusMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatusMessage.Size = new System.Drawing.Size(369, 20);
            this.txtStatusMessage.TabIndex = 9;
            // 
            // statusReseterTimer
            // 
            this.statusReseterTimer.Tick += new System.EventHandler(this.statusReseterTimer_Tick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtStatusMessage);
            this.groupBox3.Location = new System.Drawing.Point(12, 41);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(385, 46);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Status Message";
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(139, 14);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(67, 20);
            this.txtStatus.TabIndex = 11;
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 468);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MaximizeBox = false;
            this.Name = "ServerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerForm_FormClosing);
            this.Load += new System.EventHandler(this.Server_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox clientsListBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox messagesTextbox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.TextBox txtStatusMessage;
        private System.Windows.Forms.Timer statusReseterTimer;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtStatus;
    }
}

