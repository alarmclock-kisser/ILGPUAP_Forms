using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.Cuda;

namespace ILGPUAP
{
	public class GpuHandling
	{
		// ~~~~~ ~~~~~ ~~~~~ ATTRIBUTES ~~~~~ ~~~~~ ~~~~~ \\
		public int PcieId = -1;

		public Context? Ctx;
		public CudaAccelerator? Acc;
		public CudaDevice? Dev;

		public List<MemoryBuffer1D<float, Stride1D.Dense>> Buffers = [];




		// ~~~~~ ~~~~~ ~~~~~ CONSTRUCTORS ~~~~~ ~~~~~ ~~~~~ \\
		public GpuHandling()
		{

		}




		// ~~~~~ ~~~~~ ~~~~~ METHODS ~~~~~ ~~~~~ ~~~~~ \\
		// ~~~~~ Init. & setup ~~~~~ \\
		public int GetDeviceCount()
		{
			int count = CudaDevice.GetDevices(device => true).Length;
			return count;
		}

		public string[] GetDeviceNames()
		{
			var devices = CudaDevice.GetDevices(device => true);
			string[] names = new string[devices.Length];

			for (int i = 0; i < devices.Length; i++)
			{
				names[i] = devices[i].Name;
			}

			return names;
		}

		public void Init(int id = -1)
		{
			// Dispose old
			Ctx?.Dispose();
			Acc?.Dispose();
			Dev = null;
			Buffers.Clear();
			Acc?.ClearCache(ClearCacheMode.Everything);

			// Get device count & pcieid
			int count = GetDeviceCount();
			PcieId = id;

			// Abort if no devices available
			if (count == 0)
			{
				// Dispose
				Ctx?.Dispose();
				Acc?.Dispose();

				Ctx = null;
				Acc = null;
				Dev = null;
				return;
			}

			// Abort if pcieid is out of range
			if (PcieId < -1 || PcieId >= count)
			{
				// Dispose
				Ctx?.Dispose();
				Acc?.Dispose();

				// Reset to null
				Ctx = null;
				Acc = null;
				Dev = null;
				return;
			}

			// Set pcieid to 0 if -1
			if (PcieId == -1)
			{
				PcieId = 0;
			}

			// Set device
			Dev = (CudaDevice?) CudaDevice.GetDevices(device => true)[PcieId];

			// Create context
			Ctx = Context.CreateDefault();

			// Create accelerator
			Acc = Ctx.CreateCudaAccelerator(PcieId);
		}


		// ~~~~~ Info & stats ~~~~~ \\
		public string GetDeviceName()
		{
			if (Dev == null)
			{
				return "NaN";
			}

			return Dev.Name;
		}

		public string GetDeviceType()
		{
			if (Dev == null)
			{
				return "NaN";
			}

			return Dev.GetType().ToString().Split('.')[^1];
		}

		public string GetDeviceDriver()
		{
			string driver = "NaN";
			if (Dev == null)
			{
				return driver;
			}

			driver = Dev.DriverVersion.Major + "." + Dev.DriverVersion.Minor;

			return driver;
		}

		public int[] GetDeviceArchitecture()
		{
			int[] cc = [0, 0];

			if (Dev == null)
			{
				return cc;
			}

			cc[0] = Dev.Architecture.GetValueOrDefault().Major;
			cc[1] = Dev.Architecture.GetValueOrDefault().Minor;

			return cc;
		}

		public long[] GetDeviceMemory(bool readable = false)
		{
			// [0] = total memory, [1] = free memory, [2] = used memory
			long[] mem = [0, 0, 0];

			if (Dev == null || Acc == null)
			{
				return mem;
			}

			mem[0] = Dev.MemorySize;
			mem[1] = Acc.GetFreeMemory();
			mem[2] = mem[0] - mem[1];

			// Readable
			if (readable)
			{
				mem[0] = mem[0] / 1024 / 1024;
				mem[1] = mem[1] / 1024 / 1024;
				mem[2] = mem[2] / 1024 / 1024;
			}

			return mem;
		}

		public long[] GetDeviceClock(bool readable = false)
		{
			// [0] = core clock, [1] = memory clock
			long[] clock = [0, 0];

			if (Dev == null)
			{
				return clock;
			}

			clock[0] = Dev.ClockRate;
			clock[1] = Dev.MemoryClockRate;

			// Readable
			if (readable)
			{
				clock[0] = clock[0] / 1000;
				clock[1] = clock[1] / 1000;
			}

			return clock;
		}

		public int[] GetDevicePcie()
		{
			// [0] = pcie device id, [1] = pcie bus id, [2] = pcie bus width
			int[] pcie = [-1, -1,  0];

			if (Dev == null)
			{
				return pcie;
			}

			pcie[0] = Dev.PCIDeviceId;
			pcie[1] = Dev.PCIBusId;
			pcie[2] = Dev.MemoryBusWidth;

			return pcie;
		}

		public int[] GetDeviceProcessors()
		{
			// [0] = Multiprocessors, [1] = Max threads, [2] = Max threads per group, [3] = Max threads per multiprocessor, [4] = Warp size
			int[] mp = [0, 0, 0, 0, 0];

			if (Dev == null)
			{
				return mp;
			}

			mp[0] = Dev.NumMultiprocessors;
			mp[1] = Dev.MaxNumThreads;
			mp[2] = Dev.MaxNumThreadsPerGroup;
			mp[3] = Dev.MaxNumThreadsPerMultiprocessor;
			mp[4] = Dev.WarpSize;

			return mp;
		}


		// ~~~~~ Allocation of memory ~~~~~ \\
		public MemoryBuffer1D<float, Stride1D.Dense>? FindPointer(long pointer)
		{
			// Abort if no accelerator
			if (Acc == null || pointer == 0)
			{
				return null;
			}

			// Find buffer by IntPtr64 & size
			MemoryBuffer1D<float, Stride1D.Dense>? buffer = Buffers.Find(b => b.NativePtr.ToInt64() == pointer) ?? null;

			return buffer;
		}

		public long[] AllocateMemory(long length)
		{
			// [0] = IntPtr, [1] = size in bytes
			long[] mem = [0, -1];

			// Abort if no accelerator
			if (Acc == null)
			{
				return mem;
			}

			// Allocate memory
			MemoryBuffer1D<float, Stride1D.Dense> buffer = Acc.Allocate1D<float>(length);

			// Get pointer & size
			mem[0] = buffer.NativePtr.ToInt64();
			mem[1] = buffer.LengthInBytes;

			return mem;
		}

		public long FreeMemory(long pointer)
		{
			// Abort if no accelerator
			if (Acc == null || pointer == 0)
			{
				return -1;
			}

			// Find buffer by IntPtr64 & size
			MemoryBuffer1D<float, Stride1D.Dense>? buffer = FindPointer(pointer);

			// Abort if buffer not found
			if (buffer == null)
			{
				return -1;
			}

			// Get size
			long size = buffer.LengthInBytes;

			// Free memory
			buffer.Dispose();
			Buffers.Remove(buffer);

			return size;
		}

		public long[] CopyToCuda(float[] data)
		{
			// [0] = IntPtr, [1] = size in bytes
			long[] mem = [0, -1];

			// Abort if no accelerator
			if (Acc == null)
			{
				return mem;
			}

			// Allocate memory
			MemoryBuffer1D<float, Stride1D.Dense> buffer = Acc.Allocate1D<float>(data.Length);

			// Copy data
			buffer.CopyFromCPU(data);

			// Get pointer & size
			mem[0] = buffer.NativePtr.ToInt64();
			mem[1] = buffer.LengthInBytes;

			Buffers.Add(buffer);
			return mem;
		}

		public float[] CopyToHost(long pointer, bool free = false)
		{
			// Abort if no accelerator
			if (Acc == null)
			{
				return [];
			}

			// Find buffer by IntPtr64 & size
			MemoryBuffer1D<float, Stride1D.Dense>? buffer = FindPointer(pointer);
			long size = buffer?.LengthInBytes ?? -1;


			// Abort if buffer not found
			if (buffer == null || size == -1)
			{
				return [];
			}

			// Copy data
			float[] data = new float[size / sizeof(float)];
			buffer.CopyToCPU(data);

			// Free memory
			if (free)
			{
				buffer.Dispose();
				Buffers.Remove(buffer);
			}

			// Return data
			return data;
		}


		// ~~~~~ Operations on memory ~~~~~ \\


	}
}
