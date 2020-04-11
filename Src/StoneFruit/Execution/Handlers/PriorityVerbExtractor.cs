﻿using System;
using System.Collections.Generic;

namespace StoneFruit.Execution.Handlers
{
    public class PriorityVerbExtractor : ITypeVerbExtractor
    {
        private readonly ITypeVerbExtractor[] _extractors;

        public PriorityVerbExtractor(params ITypeVerbExtractor[] extractors)
        {
            _extractors = extractors;
        }

        public IReadOnlyList<string> GetVerbs(Type type)
        {
            if (!typeof(IHandlerBase).IsAssignableFrom(type))
                return new string[0];
            foreach (var extractor in _extractors)
            {
                var verbs = extractor.GetVerbs(type);
                if (verbs != null && verbs.Count > 0)
                    return verbs;
            }
            return new string[0];
        }
    }
}