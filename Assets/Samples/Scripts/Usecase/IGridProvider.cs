using Dustytoy.Samples.Grid2D.Entity;
using System;

namespace Dustytoy.Samples.Grid2D.Usecase
{
    internal interface IGridProvider
    {
        public MyCell[] GetCells();
    }
}
