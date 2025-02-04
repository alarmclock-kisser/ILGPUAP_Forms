using ManagedCuda;
using ManagedCuda.BasicTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILGPUAP
{
	public class CudaHandling
	{
		// ~~~~~ ~~~~~ ~~~~~ ATTRIBUTES ~~~~~ ~~~~~ ~~~~~ \\
		public GpuHandling GpuH;
		public ListBox? Logbox;

		public int DeviceId = -1;

		public PrimaryContext? Ctx;

		public List<CUdeviceptr> Pointers = [];


		// ~~~~~ ~~~~~ ~~~~~ CONSTRUCTORS ~~~~~ ~~~~~ ~~~~~ \\
		public CudaHandling(GpuHandling gpuhandling, ListBox? logbox = null)
		{
			GpuH = gpuhandling;
			Logbox = logbox;
		}




		// ~~~~~ ~~~~~ ~~~~~ METHODS ~~~~~ ~~~~~ ~~~~~ \\
		public void Log(string message)
		{
			if (Logbox != null)
			{
				message = "[" + DateTime.Now.ToString("HH:mm:ss:fff") + "] <CUDA> $: " + message;
				Logbox.Items.Add(message);
				Logbox.SelectedIndex = Logbox.Items.Count - 1;
			}
		}

		public void Init(int index = -1)
		{
			// Dispose old
			Ctx?.Dispose();
			Ctx = null;
			Pointers.Clear();
			Pointers = [];

			// Get device count & pcieid
			int count = CudaContext.GetDeviceCount();
			DeviceId = index;

			if (index == -1)
			{
				DeviceId = GpuH.Dev?.DeviceId ?? -1;
			}

			if (DeviceId < 0 || DeviceId >= count)
			{
				DeviceId = -2;
				Log("Could not initialize device with #" + index + " (out of bounds (max devices))");
				return;
			}

			// Create context
			Ctx = new PrimaryContext(DeviceId);
			Ctx.SetCurrent();
			Log("Initialized device with #" + index);
		}

		public void AddPointer(int pointer = -1)
		{
			CUdeviceptr ptr;
			long size;

			// If pointer is -1, add all Buffers
			if (pointer == -1)
			{
				Pointers.Clear();
				foreach (var buffer in GpuH.Buffers)
				{
					ptr = new CUdeviceptr(buffer.NativePtr);
					size = ptr.AttributeRangeSize;
					Pointers.Add(ptr);

					Log("Added buffer with pointer " + ptr.Pointer + " and size " + size);
				}
				return;
			}

			// Get pointer from long
			ptr = new(pointer);

			// Get size
			size = ptr.AttributeRangeSize;

			// LOG
			Log("Added pointer " + ptr.Pointer + " with size " + size);

			// Add to dictionary
			Pointers.Add(ptr);
		}

		public long GetPointerSize(long pointer, bool readable = false)
		{
			// Abort if no pointers or pointer is <= 0
			if (Pointers.Count == 0 || pointer <= 0)
			{
				return 0;
			}

			CUdeviceptr ptr = new(pointer);
			long size = ptr.AttributeRangeSize;

			// Readable
			if (readable)
			{
				size = size / 1024 / 1024;
			}

			return size;
		}
	}
}
