using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsExamplePv421.Models;

public interface IVisualElement
{
    public void Clear();
    public void Draw();

    //int Width { get; set; }
    //int Height { get; set; }
    //int startX { get; set; }
    //int startY { get; set; }
}
