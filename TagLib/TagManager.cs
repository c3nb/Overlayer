using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib.Tags;

namespace TagLib
{
    public static class TagManager
    {
        public static TagCollection AllTags = new TagCollection();
        public static TagCollection NotPlayingTags = new TagCollection();
    }
}
