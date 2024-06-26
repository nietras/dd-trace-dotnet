//------------------------------------------------------------------------------
// <auto-generated />
// This file was automatically generated by the UpdateVendors tool.
//------------------------------------------------------------------------------
#pragma warning disable CS0618, CS0649, CS1574, CS1580, CS1581, CS1584, CS1591, CS1573, CS8018, SYSLIB0011, SYSLIB0032
#pragma warning disable CS8600, CS8601, CS8602, CS8603, CS8604, CS8618, CS8620, CS8714, CS8762, CS8765, CS8766, CS8767, CS8768, CS8769, CS8612, CS8629, CS8774
#nullable enable
// Decompiled with JetBrains decompiler
// Type: System.Reflection.Metadata.Ecma335.NestedClassTableReader
// Assembly: System.Reflection.Metadata, Version=7.0.0.2, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2EB35F4B-CF50-496F-AFB8-CC6F6F79CB72

using Datadog.Trace.VendoredMicrosoftCode.System.Reflection.Internal;

namespace Datadog.Trace.VendoredMicrosoftCode.System.Reflection.Metadata.Ecma335
{
  internal readonly struct NestedClassTableReader
  {
    internal readonly int NumberOfRows;
    private readonly bool _IsTypeDefTableRowRefSizeSmall;
    private readonly int _NestedClassOffset;
    private readonly int _EnclosingClassOffset;
    internal readonly int RowSize;
    internal readonly MemoryBlock Block;

    internal NestedClassTableReader(
      int numberOfRows,
      bool declaredSorted,
      int typeDefTableRowRefSize,
      MemoryBlock containingBlock,
      int containingBlockOffset)
    {
      this.NumberOfRows = numberOfRows;
      this._IsTypeDefTableRowRefSizeSmall = typeDefTableRowRefSize == 2;
      this._NestedClassOffset = 0;
      this._EnclosingClassOffset = this._NestedClassOffset + typeDefTableRowRefSize;
      this.RowSize = this._EnclosingClassOffset + typeDefTableRowRefSize;
      this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, this.RowSize * numberOfRows);
      if (declaredSorted || this.CheckSorted())
        return;
      Throw.TableNotSorted(TableIndex.NestedClass);
    }

    internal TypeDefinitionHandle GetNestedClass(int rowId) => TypeDefinitionHandle.FromRowId(this.Block.PeekReference((rowId - 1) * this.RowSize + this._NestedClassOffset, this._IsTypeDefTableRowRefSizeSmall));

    internal TypeDefinitionHandle GetEnclosingClass(int rowId) => TypeDefinitionHandle.FromRowId(this.Block.PeekReference((rowId - 1) * this.RowSize + this._EnclosingClassOffset, this._IsTypeDefTableRowRefSizeSmall));

    internal TypeDefinitionHandle FindEnclosingType(TypeDefinitionHandle nestedTypeDef)
    {
      int num = this.Block.BinarySearchReference(this.NumberOfRows, this.RowSize, this._NestedClassOffset, (uint) nestedTypeDef.RowId, this._IsTypeDefTableRowRefSizeSmall);
      return num == -1 ? new TypeDefinitionHandle() : TypeDefinitionHandle.FromRowId(this.Block.PeekReference(num * this.RowSize + this._EnclosingClassOffset, this._IsTypeDefTableRowRefSizeSmall));
    }

    private bool CheckSorted() => this.Block.IsOrderedByReferenceAscending(this.RowSize, this._NestedClassOffset, this._IsTypeDefTableRowRefSizeSmall);
  }
}
