using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Vipers
{
    public class VipersInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Vipers";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return Resource1.Vipers;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("9c58b90b-d20b-4dbe-b4a3-54edda1f34f0");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "TangChi";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "QQ 1138127311  Tel 13916592695";
            }
        }
    }
}
