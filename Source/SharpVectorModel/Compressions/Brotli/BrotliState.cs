//------------------------------------------------------------------------------------
// Copyright 2015 Google Inc. All Rights Reserved. 
//
// Distributed under MIT license.
// See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
//------------------------------------------------------------------------------------

using System;
using System.IO;

namespace SharpVectors.Compressions.Brotli
{
	internal sealed class BrotliState
	{
		internal int runningState = RunningState.Uninitialized;

		internal int nextRunningState;

		internal readonly BitReader br = new BitReader();

		internal byte[] ringBuffer;

		internal readonly int[] blockTypeTrees = new int[3 * Huffman.HuffmanMaxTableSize];

		internal readonly int[] blockLenTrees = new int[3 * Huffman.HuffmanMaxTableSize];

		internal int metaBlockLength;

		internal bool inputEnd;

		internal bool isUncompressed;

		internal bool isMetadata;

		internal readonly HuffmanTreeGroup hGroup0 = new HuffmanTreeGroup();

		internal readonly HuffmanTreeGroup hGroup1 = new HuffmanTreeGroup();

		internal readonly HuffmanTreeGroup hGroup2 = new HuffmanTreeGroup();

		internal readonly int[] blockLength = new int[3];

		internal readonly int[] numBlockTypes = new int[3];

		internal readonly int[] blockTypeRb = new int[6];

		internal readonly int[] distRb = new int[] { 16, 15, 11, 4 };

		internal int pos = 0;

		internal int maxDistance = 0;

		internal int distRbIdx = 0;

		internal bool trivialLiteralContext = false;

		internal int literalTreeIndex = 0;

		internal int literalTree;

		internal int j;

		internal int insertLength;

		internal byte[] contextModes;

		internal byte[] contextMap;

		internal int contextMapSlice;

		internal int distContextMapSlice;

		internal int contextLookupOffset1;

		internal int contextLookupOffset2;

		internal int treeCommandOffset;

		internal int distanceCode;

		internal byte[] distContextMap;

		internal int numDirectDistanceCodes;

		internal int distancePostfixMask;

		internal int distancePostfixBits;

		internal int distance;

		internal int copyLength;

		internal int copyDst;

		internal int maxBackwardDistance;

		internal int maxRingBufferSize;

		internal int ringBufferSize = 0;

		internal long expectedTotalSize = 0;

		internal byte[] customDictionary = new byte[0];

		internal int bytesToIgnore = 0;

		internal int outputOffset;

		internal int outputLength;

		internal int outputUsed;

		internal int bytesWritten;

		internal int bytesToWrite;

		internal byte[] output;

		// Current meta-block header information.
		// TODO: Update to current spec.
		private static int DecodeWindowBits(BitReader br)
		{
			if (BitReader.ReadBits(br, 1) == 0)
			{
				return 16;
			}
			int n = BitReader.ReadBits(br, 3);
			if (n != 0)
			{
				return 17 + n;
			}
			n = BitReader.ReadBits(br, 3);
			if (n != 0)
			{
				return 8 + n;
			}
			return 17;
		}

		/// <summary>Associate input with decoder state.</summary>
		/// <param name="state">uninitialized state without associated input</param>
		/// <param name="input">compressed data source</param>
		internal static void SetInput(BrotliState state, Stream input)
		{
			if (state.runningState != RunningState.Uninitialized)
			{
				throw new InvalidOperationException("State MUST be uninitialized");
			}
			BitReader.Init(state.br, input);
			int windowBits = DecodeWindowBits(state.br);
			if (windowBits == 9)
			{
				/* Reserved case for future expansion. */
				throw new BrotliRuntimeException("Invalid 'windowBits' code");
			}
			state.maxRingBufferSize = 1 << windowBits;
			state.maxBackwardDistance = state.maxRingBufferSize - 16;
			state.runningState = RunningState.BlockStart;
		}

		/// <exception cref="IOException"/>
		internal static void Close(BrotliState state)
		{
			if (state.runningState == RunningState.Uninitialized)
			{
				throw new InvalidOperationException("State MUST be initialized");
			}
			if (state.runningState == RunningState.Closed)
			{
				return;
			}
			state.runningState = RunningState.Closed;
			BitReader.Close(state.br);
		}
	}
}
