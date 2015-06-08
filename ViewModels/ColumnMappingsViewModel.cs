using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;

namespace CandidateAssessment.ViewModels
{
    public class ColumnMappingsViewModel : ReactiveObject
    {
        private readonly IEnumerable<ColumnMappingViewModel> _mappings;

        public ColumnMappingsViewModel(IEnumerable<ColumnMappingViewModel> mappings)
        {
            if (mappings == null) throw new ArgumentNullException("mappings");
            _mappings = mappings;
        }

        public IEnumerable<ColumnMappingViewModel> Mappings
        {
            get { return _mappings;}
        }

        public Dictionary<string, string> MappingDictionary()
        {
            return Mappings.Select(m => m.SourceColumn).ToDictionary(sc => sc, sc => Mappings.Single(mc => mc.SourceColumn == sc).ChosenTargetColumn);
        }
    }
}