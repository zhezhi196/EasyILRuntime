using System.Collections.Generic;

namespace Module
{
    public interface ISee
    {
        List<ISensorTarget> canSeeTarget { get; }
    }
}