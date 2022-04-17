namespace AeroDiag
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.loadAeroDataButton = new System.Windows.Forms.Button();
            this.timeComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dateComboBox = new System.Windows.Forms.DateTimePicker();
            this.pointTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.aeroDataText = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.loadAeroDataButton);
            this.groupBox1.Controls.Add(this.timeComboBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dateComboBox);
            this.groupBox1.Controls.Add(this.pointTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(10, 9);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(679, 47);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Выбор Пункта и даты";
            // 
            // loadAeroDataButton
            // 
            this.loadAeroDataButton.Location = new System.Drawing.Point(487, 16);
            this.loadAeroDataButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.loadAeroDataButton.Name = "loadAeroDataButton";
            this.loadAeroDataButton.Size = new System.Drawing.Size(82, 22);
            this.loadAeroDataButton.TabIndex = 6;
            this.loadAeroDataButton.Text = "Загрузить";
            this.loadAeroDataButton.UseVisualStyleBackColor = true;
            this.loadAeroDataButton.Click += new System.EventHandler(this.loadAeroDataButton_Click);
            // 
            // timeComboBox
            // 
            this.timeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.timeComboBox.FormattingEnabled = true;
            this.timeComboBox.Items.AddRange(new object[] {
            "00",
            "06",
            "12",
            "18"});
            this.timeComboBox.Location = new System.Drawing.Point(427, 16);
            this.timeComboBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.timeComboBox.Name = "timeComboBox";
            this.timeComboBox.Size = new System.Drawing.Size(56, 23);
            this.timeComboBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(374, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Время";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(168, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Дата";
            // 
            // dateComboBox
            // 
            this.dateComboBox.Location = new System.Drawing.Point(209, 17);
            this.dateComboBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dateComboBox.Name = "dateComboBox";
            this.dateComboBox.Size = new System.Drawing.Size(161, 23);
            this.dateComboBox.TabIndex = 2;
            // 
            // pointTextBox
            // 
            this.pointTextBox.Location = new System.Drawing.Point(53, 17);
            this.pointTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pointTextBox.Name = "pointTextBox";
            this.pointTextBox.Size = new System.Drawing.Size(110, 23);
            this.pointTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Пункт";
            // 
            // aeroDataText
            // 
            this.aeroDataText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.aeroDataText.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.aeroDataText.Location = new System.Drawing.Point(16, 61);
            this.aeroDataText.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.aeroDataText.Multiline = true;
            this.aeroDataText.Name = "aeroDataText";
            this.aeroDataText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.aeroDataText.Size = new System.Drawing.Size(674, 269);
            this.aeroDataText.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 338);
            this.Controls.Add(this.aeroDataText);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Загрузка данных аэрологического зондирования";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupBox groupBox1;
        private ComboBox timeComboBox;
        private Label label3;
        private Label label2;
        private DateTimePicker dateComboBox;
        private TextBox pointTextBox;
        private Label label1;
        private TextBox aeroDataText;
        private Button loadAeroDataButton;
    }
}