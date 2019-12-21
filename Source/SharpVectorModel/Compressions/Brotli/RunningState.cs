//------------------------------------------------------------------------------------
// Copyright 2015 Google Inc. All Rights Reserved. 
//
// Distributed under MIT license.
// See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
//------------------------------------------------------------------------------------

namespace SharpVectors.Compressions.Brotli
{
	/// <summary>Enumeration of decoding state-machine.</summary>
	internal static class RunningState
	{
		public const int Uninitialized        = 0;

        public const int BlockStart           = 1;

        public const int CompressedBlockStart = 2;

        public const int MainLoop             = 3;

        public const int ReadMetadata         = 4;

        public const int CopyUncompressed     = 5;

        public const int InsertLoop           = 6;

        public const int CopyLoop             = 7;

        public const int CopyWrapBuffer       = 8;

        public const int Transform            = 9;

        public const int Finished             = 10;

        public const int Closed               = 11;

        public const int Write                = 12;
	}
}
