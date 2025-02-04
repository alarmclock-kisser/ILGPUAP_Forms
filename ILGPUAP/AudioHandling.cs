using NAudio.Wave;
using System.Drawing.Drawing2D;

namespace ILGPUAP
{
	public class AudioHandling
	{
		// ~~~~~ ~~~~~ ~~~~~ ATTRIBUTES ~~~~~ ~~~~~ ~~~~~ \\
		public List<AudioObject> Tracks = [];




		// ~~~~~ ~~~~~ ~~~~~ CONSTRUCTORS ~~~~~ ~~~~~ ~~~~~ \\
		public AudioHandling()
		{
			
		}




		// ~~~~~ ~~~~~ ~~~~~ METHODS ~~~~~ ~~~~~ ~~~~~ \\
		public void AddTrack(string filepath)
		{
			Tracks.Add(new AudioObject(filepath));
		}




	}




	public class AudioObject
	{
		// ~~~~~ ~~~~~ ~~~~~ ATTRIBUTES ~~~~~ ~~~~~ ~~~~~ \\
		public WaveOutEvent Player;

		public string Filepath;
		public string Name;

		public int Samplerate = 44100;
		public int Bitdepth = 16;
		public int Channels = 2;

		public long Length = 0;
		public double Duration = 0.0;

		public float[] Data = [];
		public long Pointer = 0;




		// ~~~~~ ~~~~~ ~~~~~ CONSTRUCTORS ~~~~~ ~~~~~ ~~~~~ \\
		public AudioObject(string filepath)
		{
			// New player
			Player = new WaveOutEvent();

			// Set filepath and name
			Filepath = filepath;
			Name = Path.GetFileNameWithoutExtension(Filepath);

			// Abort if file does not exist or isnt .wav, .mp3, .flac
			if (!File.Exists(Filepath) || !Filepath.EndsWith(".wav") && !Filepath.EndsWith(".mp3") && !Filepath.EndsWith(".flac"))
			{
				Name = "Invalid file";
				return;
			}

			// New AudioFileReader
			AudioFileReader reader = new (Filepath);

			// Set attributes
			Samplerate = reader.WaveFormat.SampleRate;
			Bitdepth = reader.WaveFormat.BitsPerSample;
			Channels = reader.WaveFormat.Channels;
			Length = reader.Length;
			Duration = reader.TotalTime.TotalSeconds;

			// Read data
			Data = new float[Length];
			int read = reader.Read(Data, 0, (int) Length);

			// Dispose reader
			reader.Dispose();
		}




		// ~~~~~ ~~~~~ ~~~~~ METHODS ~~~~~ ~~~~~ ~~~~~ \\
		public byte[] GetBytes()
		{
			int bytesPerSample = Bitdepth / 8;
			byte[] bytes = new byte[Data.Length * bytesPerSample];

			for (int i = 0; i < Data.Length; i++)
			{
				byte[] byteArray;
				float sample = Data[i];

				switch (Bitdepth)
				{
					case 16:
						short shortSample = (short) (sample * short.MaxValue);
						byteArray = BitConverter.GetBytes(shortSample);
						break;
					case 24:
						int intSample24 = (int) (sample * (1 << 23));
						byteArray = new byte[3];
						byteArray[0] = (byte) (intSample24 & 0xFF);
						byteArray[1] = (byte) ((intSample24 >> 8) & 0xFF);
						byteArray[2] = (byte) ((intSample24 >> 16) & 0xFF);
						break;
					case 32:
						int intSample32 = (int) (sample * int.MaxValue);
						byteArray = BitConverter.GetBytes(intSample32);
						break;
					default:
						throw new ArgumentException("Unsupported bit depth");
				}

				Buffer.BlockCopy(byteArray, 0, bytes, i * bytesPerSample, bytesPerSample);
			}

			return bytes;
		}

		public Bitmap DrawWaveformSmooth(PictureBox wavebox, long offset = 0, int samplesPerPixel = 1, bool update = false, Color? color = null)
		{
			// Überprüfen, ob floats und die PictureBox gültig sind
			if (Data.Length == 0 || wavebox.Width <= 0 || wavebox.Height <= 0)
			{
				// Empty picturebox
				if (update)
				{
					wavebox.Image = null;
					wavebox.Refresh();
				}

				return new Bitmap(1, 1);
			}

			// Colors
			Color waveformColor = color ?? Color.FromName("HotTrack");
			Color backgroundColor = Color.White;

			Bitmap bmp = new(wavebox.Width, wavebox.Height);
			using Graphics gfx = Graphics.FromImage(bmp);
			using Pen pen = new(waveformColor);
			gfx.SmoothingMode = SmoothingMode.AntiAlias;
			gfx.Clear(backgroundColor);

			float centerY = wavebox.Height / 2f;
			float yScale = wavebox.Height / 2f;

			for (int x = 0; x < wavebox.Width; x++)
			{
				long sampleIndex = offset + (long) x * samplesPerPixel;

				if (sampleIndex >= Data.Length)
				{
					break;
				}

				float maxValue = float.MinValue;
				float minValue = float.MaxValue;

				for (int i = 0; i < samplesPerPixel; i++)
				{
					if (sampleIndex + i < Data.Length)
					{
						maxValue = Math.Max(maxValue, Data[sampleIndex + i]);
						minValue = Math.Min(minValue, Data[sampleIndex + i]);
					}
				}

				float yMax = centerY - maxValue * yScale;
				float yMin = centerY - minValue * yScale;

				// Überprüfen, ob die Werte innerhalb des sichtbaren Bereichs liegen
				if (yMax < 0) yMax = 0;
				if (yMin > wavebox.Height) yMin = wavebox.Height;

				// Zeichne die Linie nur, wenn sie sichtbar ist
				if (Math.Abs(yMax - yMin) > 0.01f)
				{
					gfx.DrawLine(pen, x, yMax, x, yMin);
				}
				else if (samplesPerPixel == 1)
				{
					// Zeichne einen Punkt, wenn samplesPerPixel 1 ist und die Linie zu klein ist
					gfx.DrawLine(pen, x, centerY, x, centerY - Data[sampleIndex] * yScale);
				}
			}

			// Update PictureBox
			if (update)
			{
				wavebox.Image = bmp;
				wavebox.Refresh();
			}

			return bmp;
		}

		public int GetFitResolution(int width)
		{
			// Gets pixels per sample for a given width to fit the whole waveform
			int samplesPerPixel = (int) Math.Ceiling((double) Data.Length / width) / 4;
			return samplesPerPixel;
		}


	}
}
