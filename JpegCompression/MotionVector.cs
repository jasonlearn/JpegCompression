using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression
{
    /// <summary>
    /// MotionVector, holds the (x,y) of the origin, (u,v) of the change
    /// </summary>
    public class MotionVector
    {
        /// <summary>
        /// x of the origin
        /// </summary>
        public int x;

        /// <summary>
        /// y of the origin
        /// </summary>
        public int y;

        /// <summary>
        /// x of the change
        /// </summary>
        public int u;

        /// <summary>
        /// y of the change
        /// </summary>
        public int v;

        /// <summary>
        /// Constructor. No changes
        /// </summary>
        public MotionVector()
        {
            //x = 0; y = 0; u = 0; v = 0;
        }

        /// <summary>
        /// Constructor. Initializes data
        /// </summary>
        /// <param name="xx">x of the origin</param>
        /// <param name="yy">y of the origin</param>
        /// <param name="uu">x of the change</param>
        /// <param name="vv">y of the change</param>
        public MotionVector(int xx, int yy, int uu, int vv)
        {
            x = xx;
            y = yy;
            u = uu;
            v = vv;
        }
    }
}
