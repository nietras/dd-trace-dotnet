//------------------------------------------------------------------------------
// <auto-generated />
// This file was automatically generated by the UpdateVendors tool.
//------------------------------------------------------------------------------
#pragma warning disable CS0618, CS0649, CS1574, CS1580, CS1581, CS1584, CS1591, CS1573, CS8018, SYSLIB0011, SYSLIB0032
#pragma warning disable CS8600, CS8601, CS8602, CS8603, CS8604, CS8618, CS8620, CS8714, CS8762, CS8765, CS8766, CS8767, CS8768, CS8769, CS8612, CS8629, CS8774
#nullable enable
// Decompiled with JetBrains decompiler
// Type: System.Buffers.ReadOnlySequenceDebugView`1
// Assembly: System.Memory, Version=4.0.1.2, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
// MVID: 805945F3-27B0-47AD-B8F6-389D9D8F82C3

using System;
using System.Diagnostics;
using Datadog.Trace.VendoredMicrosoftCode.System.Diagnostics;

namespace Datadog.Trace.VendoredMicrosoftCode.System.Buffers
{
    internal sealed class ReadOnlySequenceDebugView<T>
  {
    private readonly T[] _array;
    private readonly ReadOnlySequenceDebugView<T>.ReadOnlySequenceDebugViewSegments _segments;

    public ReadOnlySequenceDebugView(ReadOnlySequence<T> sequence)
    {
      this._array = sequence.ToArray<T>();
      int length = 0;
      foreach (ReadOnlyMemory<T> readOnlyMemory in sequence)
        ++length;
      ReadOnlyMemory<T>[] readOnlyMemoryArray = new ReadOnlyMemory<T>[length];
      int index = 0;
      foreach (ReadOnlyMemory<T> readOnlyMemory in sequence)
      {
        readOnlyMemoryArray[index] = readOnlyMemory;
        ++index;
      }
      this._segments = new ReadOnlySequenceDebugView<T>.ReadOnlySequenceDebugViewSegments()
      {
        Segments = readOnlyMemoryArray
      };
    }

    public ReadOnlySequenceDebugView<T>.ReadOnlySequenceDebugViewSegments BufferSegments => this._segments;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items => this._array;

    [DebuggerDisplay("Count: {Segments.Length}", Name = "Segments")]
    internal struct ReadOnlySequenceDebugViewSegments
    {
      [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
      public ReadOnlyMemory<T>[] Segments { get; set; }
    }
  }
}
