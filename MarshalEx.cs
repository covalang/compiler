using System;
using System.Runtime.InteropServices;

namespace Compiler
{
	public static class MarshalEx
	{
		private static readonly Boolean IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
		private static readonly Boolean IsMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
		private static readonly Boolean IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

		public static void FreeHGlobalAligned(IntPtr intPtr)
		{
			if (IsWindows)
				_aligned_free(intPtr);
			else if (IsMacOS | IsLinux)
				free(intPtr);
			else
				throw new PlatformNotSupportedException();
		}

		public static IntPtr AllocHGlobalAligned(int size, int alignment)
		{
			if (size <= 0)
				throw new ArgumentOutOfRangeException(nameof(size));
			if (alignment <= 0)
				throw new ArgumentOutOfRangeException(nameof(alignment));
			var p = AlignedMalloc((UIntPtr)size, (UIntPtr)alignment);
			if (p == IntPtr.Zero)
				throw new Exception("Aligned malloc failed.");
			if ((UInt64)p % (UInt64)alignment != 0)
				throw new Exception("Aligned malloc returned memory that is not aligned to " + alignment + " bytes.");
			return p;
		}

		private static unsafe IntPtr AlignedMalloc(UIntPtr size, UIntPtr alignment)
		{
			if (IsWindows)
				return _aligned_malloc(size, alignment);
			if (IsLinux | IsMacOS)
			{
				IntPtr p;
				posix_memalign(&p, alignment, size);
				return p;
			}
			throw new PlatformNotSupportedException();
		}

		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr _aligned_malloc(UIntPtr size, UIntPtr alignment);

		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr _aligned_free(IntPtr memblock);

		[DllImport("libc")]
		private static extern unsafe IntPtr posix_memalign(IntPtr* memptr, UIntPtr alignment, UIntPtr size);

		[DllImport("libc")]
		private static extern void free(IntPtr ptr);
	}
}
