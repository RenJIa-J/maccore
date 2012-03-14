// 
// CMFormatDescription.cs: Implements the managed CMFormatDescription
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//   Frank Krueger
//   Mono Team
//     
// Copyright 2010 Novell, Inc
// Copyright 2012 Xamarin Inc
//
using System;
using System.Runtime.InteropServices;

using MonoMac;
using MonoMac.Foundation;
using MonoMac.CoreFoundation;
using MonoMac.ObjCRuntime;
#if !COREBUILD
using MonoMac.AudioToolbox;
#endif

namespace MonoMac.CoreMedia {

	[Since (4,0)]
	public class CMFormatDescription : INativeObject, IDisposable {
		internal IntPtr handle;

		internal CMFormatDescription (IntPtr handle)
		{
			this.handle = handle;
		}

		[Preserve (Conditional=true)]
		internal CMFormatDescription (IntPtr handle, bool owns)
		{
			if (!owns)
				CFObject.CFRetain (handle);

			this.handle = handle;
		}
		
		~CMFormatDescription ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}
	
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}
		
		/*[DllImport(Constants.CoreMediaLibrary)]
		extern static CFPropertyListRef CMFormatDescriptionGetExtension (
		   CMFormatDescriptionRef desc,
		   CFStringRef extensionKey
		);*/
		
		[DllImport(Constants.CoreMediaLibrary)]
		extern static IntPtr CMFormatDescriptionGetExtensions (IntPtr handle);

#if !COREBUILD
		
		public NSDictionary GetExtensions ()
		{
			var cfDictRef = CMFormatDescriptionGetExtensions (handle);
			if (cfDictRef == IntPtr.Zero)
			{
				return null;
			}
			else
			{
				return (NSDictionary) Runtime.GetNSObject (cfDictRef);
			}
		}

#endif
		
		[DllImport(Constants.CoreMediaLibrary)]
		extern static uint CMFormatDescriptionGetMediaSubType (IntPtr handle);
		
		public uint MediaSubType
		{
			get
			{
				return CMFormatDescriptionGetMediaSubType (handle);
			}
		}
		
		[DllImport(Constants.CoreMediaLibrary)]
		extern static CMMediaType CMFormatDescriptionGetMediaType (IntPtr handle);
		
		public CMMediaType MediaType
		{
			get
			{
				return CMFormatDescriptionGetMediaType (handle);
			}
		}
		
		[DllImport(Constants.CoreMediaLibrary)]
		extern static int CMFormatDescriptionGetTypeID ();
		
		public static int GetTypeID ()
		{
			return CMFormatDescriptionGetTypeID ();
		}
#if !COREBUILD
		[DllImport (Constants.CoreMediaLibrary)]
		extern static IntPtr CMAudioFormatDescriptionGetStreamBasicDescription (IntPtr handle);

		public AudioStreamBasicDescription? AudioStreamBasicDescription {
			get {
				var ret = CMAudioFormatDescriptionGetStreamBasicDescription (handle);
				if (ret != IntPtr.Zero){
					unsafe {
						return *((AudioStreamBasicDescription *) ret);
					}
				}
				return null;
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static IntPtr CMAudioFormatDescriptionGetChannelLayout (IntPtr handle, out int size);
			
		public AudioChannelLayout AudioChannelLayout {
			get {
				int size;
				var res = CMAudioFormatDescriptionGetChannelLayout (handle, out size);
				if (res == IntPtr.Zero)
					return null;
				return AudioFile.AudioChannelLayoutFromHandle (handle);
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static IntPtr CMAudioFormatDescriptionGetFormatList (IntPtr handle, out int size);
		public AudioFormat [] AudioFormats {
			get {
				unsafe {
					int size;
					var v = CMAudioFormatDescriptionGetFormatList (handle, out size);
					if (v == IntPtr.Zero)
						return null;
					var items = size / sizeof (AudioFormat);
					var ret = new AudioFormat [items];
					var ptr = (AudioFormat *) v;
					for (int i = 0; i < items; i++)
						ret [i] = ptr [i];
					return ret;
				}
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static IntPtr CMAudioFormatDescriptionGetMagicCookie (IntPtr handle, out int size);

		public byte [] AudioMagicCookie {
			get {
				int size;
				var h = CMAudioFormatDescriptionGetMagicCookie (handle, out size);
				if (h == IntPtr.Zero)
					return null;

				var result = new byte [size];
				for (int i = 0; i < size; i++)
					result [i] = Marshal.ReadByte (h, i);
				return result;
			}
		}
#endif
	}
}
