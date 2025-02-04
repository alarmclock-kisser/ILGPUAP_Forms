namespace ILGPUAP
{
    partial class WindowMain
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
			listBox_log = new ListBox();
			comboBox_devices = new ComboBox();
			progressBar_vram = new ProgressBar();
			label_vram = new Label();
			listBox_deviceInfo = new ListBox();
			pictureBox_waveform = new PictureBox();
			listBox_tracks = new ListBox();
			label_trackInfo = new Label();
			listBox_pointers = new ListBox();
			button_move = new Button();
			button_color = new Button();
			groupBox_settings = new GroupBox();
			label_zoom = new Label();
			label_offset = new Label();
			numericUpDown_zoom = new NumericUpDown();
			numericUpDown_offset = new NumericUpDown();
			((System.ComponentModel.ISupportInitialize) pictureBox_waveform).BeginInit();
			groupBox_settings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize) numericUpDown_zoom).BeginInit();
			((System.ComponentModel.ISupportInitialize) numericUpDown_offset).BeginInit();
			SuspendLayout();
			// 
			// listBox_log
			// 
			listBox_log.FormattingEnabled = true;
			listBox_log.ItemHeight = 15;
			listBox_log.Location = new Point(12, 825);
			listBox_log.Name = "listBox_log";
			listBox_log.Size = new Size(680, 124);
			listBox_log.TabIndex = 0;
			// 
			// comboBox_devices
			// 
			comboBox_devices.FormattingEnabled = true;
			comboBox_devices.Location = new Point(12, 12);
			comboBox_devices.Name = "comboBox_devices";
			comboBox_devices.Size = new Size(250, 23);
			comboBox_devices.TabIndex = 1;
			comboBox_devices.SelectedIndexChanged += comboBox_devices_SelectedIndexChanged;
			// 
			// progressBar_vram
			// 
			progressBar_vram.Location = new Point(12, 41);
			progressBar_vram.Name = "progressBar_vram";
			progressBar_vram.Size = new Size(250, 10);
			progressBar_vram.TabIndex = 2;
			// 
			// label_vram
			// 
			label_vram.AutoSize = true;
			label_vram.Location = new Point(12, 54);
			label_vram.Name = "label_vram";
			label_vram.Size = new Size(90, 15);
			label_vram.TabIndex = 3;
			label_vram.Text = "VRAM: 0 / 0 MB";
			// 
			// listBox_deviceInfo
			// 
			listBox_deviceInfo.FormattingEnabled = true;
			listBox_deviceInfo.ItemHeight = 15;
			listBox_deviceInfo.Items.AddRange(new object[] { "<Type> \"NaN\"", "Core: 0 MHz", "Mem: 0 MHz", "Free: 0 MB", "PCIe: #-1, #-1,  0 bits", "MPs: 0 Multis, 0 Threads, 0 warps", "Driver: Ver. 0.0 [0.0]" });
			listBox_deviceInfo.Location = new Point(12, 72);
			listBox_deviceInfo.Name = "listBox_deviceInfo";
			listBox_deviceInfo.SelectionMode = SelectionMode.None;
			listBox_deviceInfo.Size = new Size(250, 109);
			listBox_deviceInfo.TabIndex = 4;
			// 
			// pictureBox_waveform
			// 
			pictureBox_waveform.BackColor = Color.White;
			pictureBox_waveform.Location = new Point(12, 719);
			pictureBox_waveform.Name = "pictureBox_waveform";
			pictureBox_waveform.Size = new Size(680, 100);
			pictureBox_waveform.TabIndex = 5;
			pictureBox_waveform.TabStop = false;
			// 
			// listBox_tracks
			// 
			listBox_tracks.FormattingEnabled = true;
			listBox_tracks.ItemHeight = 15;
			listBox_tracks.Location = new Point(12, 424);
			listBox_tracks.Name = "listBox_tracks";
			listBox_tracks.ScrollAlwaysVisible = true;
			listBox_tracks.Size = new Size(250, 289);
			listBox_tracks.TabIndex = 6;
			listBox_tracks.SelectedIndexChanged += listBox_tracks_SelectedIndexChanged;
			// 
			// label_trackInfo
			// 
			label_trackInfo.AutoSize = true;
			label_trackInfo.Location = new Point(268, 698);
			label_trackInfo.Name = "label_trackInfo";
			label_trackInfo.Size = new Size(260, 15);
			label_trackInfo.TabIndex = 7;
			label_trackInfo.Text = "Track info (Samplerate, Bitdepth, Channels, etc.)";
			// 
			// listBox_pointers
			// 
			listBox_pointers.FormattingEnabled = true;
			listBox_pointers.ItemHeight = 15;
			listBox_pointers.Location = new Point(492, 12);
			listBox_pointers.Name = "listBox_pointers";
			listBox_pointers.SelectionMode = SelectionMode.None;
			listBox_pointers.Size = new Size(200, 169);
			listBox_pointers.TabIndex = 8;
			// 
			// button_move
			// 
			button_move.Location = new Point(268, 424);
			button_move.Name = "button_move";
			button_move.Size = new Size(75, 23);
			button_move.TabIndex = 9;
			button_move.Text = "-> CUDA";
			button_move.UseVisualStyleBackColor = true;
			button_move.Click += button_move_Click;
			// 
			// button_color
			// 
			button_color.BackColor = SystemColors.HotTrack;
			button_color.ForeColor = Color.White;
			button_color.Location = new Point(149, 22);
			button_color.Name = "button_color";
			button_color.Size = new Size(45, 23);
			button_color.TabIndex = 10;
			button_color.Text = "Color";
			button_color.UseVisualStyleBackColor = false;
			button_color.Click += button_color_Click;
			// 
			// groupBox_settings
			// 
			groupBox_settings.Controls.Add(label_zoom);
			groupBox_settings.Controls.Add(label_offset);
			groupBox_settings.Controls.Add(numericUpDown_zoom);
			groupBox_settings.Controls.Add(numericUpDown_offset);
			groupBox_settings.Controls.Add(button_color);
			groupBox_settings.Location = new Point(492, 187);
			groupBox_settings.Name = "groupBox_settings";
			groupBox_settings.Size = new Size(200, 200);
			groupBox_settings.TabIndex = 11;
			groupBox_settings.TabStop = false;
			groupBox_settings.Text = "Settings";
			// 
			// label_zoom
			// 
			label_zoom.AutoSize = true;
			label_zoom.Location = new Point(63, 144);
			label_zoom.Name = "label_zoom";
			label_zoom.Size = new Size(45, 15);
			label_zoom.TabIndex = 12;
			label_zoom.Text = "Zoom: ";
			// 
			// label_offset
			// 
			label_offset.AutoSize = true;
			label_offset.Location = new Point(23, 173);
			label_offset.Name = "label_offset";
			label_offset.Size = new Size(45, 15);
			label_offset.TabIndex = 12;
			label_offset.Text = "Offset: ";
			// 
			// numericUpDown_zoom
			// 
			numericUpDown_zoom.Location = new Point(114, 142);
			numericUpDown_zoom.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
			numericUpDown_zoom.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			numericUpDown_zoom.Name = "numericUpDown_zoom";
			numericUpDown_zoom.Size = new Size(80, 23);
			numericUpDown_zoom.TabIndex = 12;
			numericUpDown_zoom.Value = new decimal(new int[] { 1024, 0, 0, 0 });
			numericUpDown_zoom.ValueChanged += numericUpDown_zoom_ValueChanged;
			// 
			// numericUpDown_offset
			// 
			numericUpDown_offset.ImeMode = ImeMode.NoControl;
			numericUpDown_offset.Increment = new decimal(new int[] { 8196, 0, 0, 0 });
			numericUpDown_offset.Location = new Point(74, 171);
			numericUpDown_offset.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
			numericUpDown_offset.Name = "numericUpDown_offset";
			numericUpDown_offset.Size = new Size(120, 23);
			numericUpDown_offset.TabIndex = 12;
			numericUpDown_offset.ValueChanged += numericUpDown_offset_ValueChanged;
			// 
			// WindowMain
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(704, 961);
			Controls.Add(groupBox_settings);
			Controls.Add(button_move);
			Controls.Add(listBox_pointers);
			Controls.Add(label_trackInfo);
			Controls.Add(listBox_tracks);
			Controls.Add(pictureBox_waveform);
			Controls.Add(listBox_deviceInfo);
			Controls.Add(label_vram);
			Controls.Add(progressBar_vram);
			Controls.Add(comboBox_devices);
			Controls.Add(listBox_log);
			MaximumSize = new Size(720, 1000);
			MinimumSize = new Size(720, 1000);
			Name = "WindowMain";
			Text = "ILGPU AP (Forms)";
			((System.ComponentModel.ISupportInitialize) pictureBox_waveform).EndInit();
			groupBox_settings.ResumeLayout(false);
			groupBox_settings.PerformLayout();
			((System.ComponentModel.ISupportInitialize) numericUpDown_zoom).EndInit();
			((System.ComponentModel.ISupportInitialize) numericUpDown_offset).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private ListBox listBox_log;
		private ComboBox comboBox_devices;
		private ProgressBar progressBar_vram;
		private Label label_vram;
		private ListBox listBox_deviceInfo;
		private PictureBox pictureBox_waveform;
		private ListBox listBox_tracks;
		private Label label_trackInfo;
		private ListBox listBox_pointers;
		private Button button_move;
		private Button button_color;
		private GroupBox groupBox_settings;
		private Label label_zoom;
		private Label label_offset;
		private NumericUpDown numericUpDown_zoom;
		private NumericUpDown numericUpDown_offset;
	}
}
