using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    public class VariableDependency
    {
        public string VariableID { get; set; }
        public List<VariableDependency> Dependencies { get; set; }

        private List<string> _DependsOnVariables;
        public List<string> DependsOnVariables
        {
            get
            {
                if (this._DependsOnVariables.Count <= 0)
                {
                    _DependsOnVariables = this.Dependencies.Select(variableDebendency => variableDebendency.VariableID).ToList();
                    List<string> innerDependencies = this.GetInnerDependencies;
                    if (innerDependencies.Count > 0)
                    {
                        _DependsOnVariables.AddRange(innerDependencies);
                    }
                }
                return _DependsOnVariables;
            }
        }

        private List<string> GetInnerDependencies
        {
            get
            {
                List<string> innerDependencies = new List<string>();
                foreach (VariableDependency dependency in Dependencies)
                {
                    List<string> currentLevelDependencies = dependency.DependsOnVariables;
                    if (currentLevelDependencies.Count > 0)
                    {
                        innerDependencies.AddRange(currentLevelDependencies);
                    }
                    
                    foreach(VariableDependency innerVariableDependency in dependency.Dependencies)
                    {
                        currentLevelDependencies = innerVariableDependency.DependsOnVariables;
                        if (currentLevelDependencies.Count > 0)
                        {
                            innerDependencies.AddRange(currentLevelDependencies);
                        }
                    }
                }
                return innerDependencies;
            }
        }

        public VariableDependency(string variableID, List<VariableDependency> variableDependencies)
        {
            VariableID = variableID;
            Dependencies = variableDependencies;
            _DependsOnVariables = new List<string>();
        }

        public bool HasCircularReference
        {
            get
            {
                return DependsOnVariables.Contains(VariableID);
            }
        }

        public bool HasDependencies
        {
            get
            {
                return this._DependsOnVariables != null && this._DependsOnVariables.Count() > 0;
            }
        }

        public void RemoveDependencies(List<string> variableIDs)
        {
            if (this._DependsOnVariables != null)
            {
                foreach (string variableID in variableIDs)
                {
                    this._DependsOnVariables.Remove(variableID);
                }
            }
        }

        public string Info
        {
            get
            {
                return String.Join(",", this.DependsOnVariables.Distinct());
            }
        }
    }
}
