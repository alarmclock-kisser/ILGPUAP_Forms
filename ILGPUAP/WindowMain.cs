using System.Runtime.InteropServices.Marshalling;

namespace ILGPUAP
{
	public partial class WindowMain : Form
	{
		// ~~~~~ ~~~~~ ~~~~~ ATTRIBUTES ~~~~~ ~~~~~ ~~~~~ \\
		public AudioHandling AudioH;
		public GpuHandling GpuH;
		public CudaHandling CudaH;



		private int _lastZoom = 1024;


		// ~~~~~ ~~~~~ ~~~~~ CONSTRUCTORS ~~~~~ ~~~~~ ~~~~~ \\
		public WindowMain()
		{
			InitializeComponent();

			// Window position
			StartPosition = FormStartPosition.Manual;
			Location = new Point(0, 0);

			// Init. classes
			AudioH = new AudioHandling();
			GpuH = new GpuHandling();
			CudaH = new CudaHandling(GpuH, listBox_log);

			// Register events
			listBox_log.MouseDoubleClick += (sender, e) => ExportLog();
			listBox_tracks.MouseDoubleClick += (sender, e) => ImportAudios();

			// Setup GUI
			FillDevicesBox();
			UpdateGpuInfo();
		}





		// ~~~~~ ~~~~~ ~~~~~ METHODS ~~~~~ ~~~~~ ~~~~~ \\
		public void Log(string message = "", int level = 0)
		{
			string timestamp = "[" + DateTime.Now.ToString("HH:mm:ss:fff") + "]";
			string prefix = " <Host> $: ";
			listBox_log.Items.Add(timestamp + prefix + message);

			// Scroll to bottom
			listBox_log.TopIndex = listBox_log.Items.Count - 1;
		}

		public void FillDevicesBox()
		{
			// Clear box
			comboBox_devices.Items.Clear();


			// Fill box with device names
			string[] names = GpuH.GetDeviceNames();
			foreach (string name in names)
			{
				comboBox_devices.Items.Add(name);
			}

			// Add de-initialize option
			comboBox_devices.Items.Add("(!) De-initialize every CUDA device (!)");

			// Unselect
			comboBox_devices.SelectedIndex = 0;

			// LOG
			Log("Found " + names.Length + " CUDA device(s).");
		}

		public void UpdateGpuInfo()
		{
			// Clear info box
			listBox_deviceInfo.Items.Clear();

			// Get device info strings
			string name = "<" + GpuH.GetDeviceType() + "> '" + GpuH.GetDeviceName() + "'";
			string core = "Core: " + GpuH.GetDeviceClock()[0] + " MHz";
			string mem = "Mem: " + GpuH.GetDeviceClock()[1] + " MHz";
			string free = "Free: " + GpuH.GetDeviceMemory(true)[1] + " MB";
			string pcie = "PCIe: #" + GpuH.GetDevicePcie()[0] + ", #" + GpuH.GetDevicePcie()[1] + ",  " + GpuH.GetDevicePcie()[2] + " bits";
			string threads = "MPs: " + GpuH.GetDeviceProcessors()[0] + " Multis, " + GpuH.GetDeviceProcessors()[1] + " Threads, " + GpuH.GetDeviceProcessors()[4] + " warps";
			string driver = "Driver: Ver. " + GpuH.GetDeviceDriver() + " [Cap.: " + GpuH.GetDeviceArchitecture()[0] + "." + GpuH.GetDeviceArchitecture()[1] + "]";

			// Fill info listbox
			listBox_deviceInfo.Items.Add(name);
			listBox_deviceInfo.Items.Add(core);
			listBox_deviceInfo.Items.Add(mem);
			listBox_deviceInfo.Items.Add(free);
			listBox_deviceInfo.Items.Add(pcie);
			listBox_deviceInfo.Items.Add(threads);
			listBox_deviceInfo.Items.Add(driver);

			// Update VRAM info elements
			progressBar_vram.Maximum = (int) GpuH.GetDeviceMemory(true)[0];
			progressBar_vram.Value = (int) GpuH.GetDeviceMemory(true)[2];
			label_vram.Text = "VRAM: " + GpuH.GetDeviceMemory(true)[2] + " / " + GpuH.GetDeviceMemory(true)[0] + " MB";

			// LOG
			Log("Updated device info & stats.");
		}

		public void UpdateTrackView()
		{
			// Find track by name ?? -1 if not found
			int index = AudioH.Tracks.FindIndex(track => track.Name == listBox_tracks.SelectedItem?.ToString());

			// Abort if no track selected
			if (index == -1)
			{
				// Update label & waveform
				label_trackInfo.Text = "No track selected.";
				pictureBox_waveform.Image = null;

				return;
			}

			// Change move button text
			if (AudioH.Tracks[index].Pointer == 0)
			{
				button_move.Text = "-> CUDA";
			}
			else
			{
				button_move.Text = "Host <-";
			}

			// Set attribute string
			label_trackInfo.Text = AudioH.Tracks[index].Samplerate + " Hz, " + AudioH.Tracks[index].Bitdepth + " bits, " + AudioH.Tracks[index].Channels + "Ch., " + AudioH.Tracks[index].Duration + "s, " + AudioH.Tracks[index].Length + " samples";

			// Draw waveform
			numericUpDown_offset.Maximum = AudioH.Tracks[index].Length;
			numericUpDown_zoom.Maximum = AudioH.Tracks[index].GetFitResolution(pictureBox_waveform.Width);

			AudioH.Tracks[index].DrawWaveformSmooth(pictureBox_waveform, Math.Max(0, (long)numericUpDown_offset.Value), (int) numericUpDown_zoom.Value, true, button_color.BackColor);
		}

		public string ExportLog()
		{
			// Get log items.Text -> string[]
			string[] log = new string[listBox_log.Items.Count];
			listBox_log.Items.CopyTo(log, 0);

			// Write log to TXT file at Desktop with ms timestamp
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $@"\ILGPUAP_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
			File.WriteAllLines(path, log);

			// LOG
			Log("Exported log to " + path + ".");

			return path;
		}

		public void FillTrackList()
		{
			// Clear list
			listBox_tracks.Items.Clear();

			// Fill list with track names
			foreach (AudioObject track in AudioH.Tracks)
			{
				listBox_tracks.Items.Add(track.Name);
			}

			// LOG
			Log("Filled track list with " + AudioH.Tracks.Count + " track(s).");

			// Unselect
			listBox_tracks.SelectedIndex = -1;
		}

		public void FillPointerList()
		{
			// Clear list
			listBox_pointers.Items.Clear();

			// Add all pointers using ManagedCuda
			CudaH.AddPointer();

			// Fill list with pointer names if theyre in CUDA
			foreach (var ptr in GpuH.Buffers)
			{
				long pointer = ptr.NativePtr.ToInt64();
				long size;
				if (GpuH.FindPointer(pointer)?.NativePtr == pointer)
				{
					size = CudaH.GetPointerSize(pointer, true);
					listBox_pointers.Items.Add("<" + pointer + "> (" + size + " MB)");
				}
				else
				{
					listBox_pointers.Items.Add("! " + pointer + " !" + "(NaN MB)");
					Log("Pointer " + pointer + " not found in CUDA memory.", 1);
				}

			}

			// LOG
			Log("Filled pointer list with " + CudaH.Pointers.Count + " pointer(s).");
		}

		public void ImportAudios()
		{
			// OFD
			OpenFileDialog ofd = new();
			ofd.Title = "Select audio files to import.";
			ofd.Filter = "Audio files|*.wav;*.mp3;*.flac";
			ofd.Multiselect = true;
			ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
			ofd.CheckFileExists = true;

			// Show OFD
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				// Add tracks
				foreach (string file in ofd.FileNames)
				{
					AudioH.AddTrack(file);
				}

				// Update GUI
				FillTrackList();
			}
		}




		// ~~~~~ ~~~~~ ~~~~~ EVENTS ~~~~~ ~~~~~ ~~~~~ \\
		private void comboBox_devices_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Init. device with selected index as id using ILGPU
			GpuH.Init(comboBox_devices.SelectedIndex);

			// Init. device with selected index as id using ManagedCuda
			CudaH.Init(comboBox_devices.SelectedIndex);

			// LOG
			Log("Initialized " + GpuH.GetDeviceName() + " as device " + comboBox_devices.SelectedIndex + ".");

			// Update GUI
			UpdateGpuInfo();
		}

		private void listBox_tracks_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Reset zoom & offset
			numericUpDown_zoom.Value = 1024;
			numericUpDown_offset.Value = 0;

			// Update track view
			UpdateTrackView();
		}

		private void button_move_Click(object sender, EventArgs e)
		{
			// Abort if no device initialized
			if (GpuH.Dev == null)
			{
				Log("Couldn't move audio data: No CUDA device initialized.", 1);
				return;
			}

			// Find track by name ?? -1 if not found
			int index = AudioH.Tracks.FindIndex(track => track.Name == listBox_tracks.SelectedItem?.ToString());

			// Abort if no track selected
			if (index == -1)
			{
				Log("Couldn't move audio data: No track selected.", 1);
				return;
			}

			// If track is already in CUDA
			if (AudioH.Tracks[index].Pointer != 0)
			{
				// Move track to Host (free memory)
				AudioH.Tracks[index].Data = GpuH.CopyToHost(AudioH.Tracks[index].Pointer, true);

				// Clear track pointer
				AudioH.Tracks[index].Pointer = 0;

				// LOG
				Log("Moved audio data of track '" + AudioH.Tracks[index].Name + "' -> Host memory.", 0);

				// Update GUI
				FillPointerList();
				UpdateTrackView();
				UpdateGpuInfo();
			}
			else
			{
				// Move track to CUDA (allocate memory)
				long[] buffer = GpuH.CopyToCuda(AudioH.Tracks[index].Data);

				// Clear track data & set pointer
				AudioH.Tracks[index].Data = [];
				AudioH.Tracks[index].Pointer = buffer[0];

				// LOG
				Log("Moved audio data of track '" + AudioH.Tracks[index].Name + "' -> CUDA memory.", 0);

				// Update GUI
				FillPointerList();
				UpdateTrackView();
				UpdateGpuInfo();
			}
		}

		private void button_color_Click(object sender, EventArgs e)
		{
			// Open color dialog -> set button back color (+ toggle text color if too dark)
			ColorDialog cd = new();
			if (cd.ShowDialog() == DialogResult.OK)
			{
				button_color.BackColor = cd.Color;

				// Toggle text color if too dark
				if (cd.Color.GetBrightness() < 0.4)
				{
					button_color.ForeColor = Color.White;
				}
				else
				{
					button_color.ForeColor = Color.Black;
				}
			}

			// Update track view
			UpdateTrackView();
		}

		private void numericUpDown_zoom_ValueChanged(object? sender, EventArgs e)
		{
			// Get current zoom value
			int currentZoom = (int) numericUpDown_zoom.Value;

			// If increased: double value
			if (currentZoom > _lastZoom)
			{
				currentZoom = (int) (currentZoom * 1.5f);
				if (currentZoom >= numericUpDown_zoom.Maximum)
				{
					currentZoom = (int) numericUpDown_zoom.Maximum;
				}
			}

			// If decreased: half value
			else if (currentZoom < _lastZoom)
			{
				currentZoom = (int) (currentZoom / 1.5f);
				if (currentZoom <= numericUpDown_zoom.Minimum)
				{
					currentZoom = (int) numericUpDown_zoom.Minimum;
				}
			}

			// Temporarily remove the event handler to prevent recursion
			numericUpDown_zoom.ValueChanged -= numericUpDown_zoom_ValueChanged;

			// Update zoom value
			numericUpDown_zoom.Value = currentZoom;

			// Reattach the event handler
			numericUpDown_zoom.ValueChanged += numericUpDown_zoom_ValueChanged;

			// Update last zoom
			_lastZoom = currentZoom;

			// Update track view
			UpdateTrackView();
		}

		private void numericUpDown_offset_ValueChanged(object? sender, EventArgs e)
		{
			// Update track view
			UpdateTrackView();
		}

	}
}
