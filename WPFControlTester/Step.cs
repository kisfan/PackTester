using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackTesterInterface
{
    public enum stepType
    {
        Discharge,
        Charge,
        Wait,
    };

    public enum stepStatus
    {
        Completed,
        Incompleted,
        Selected,
    };

    public class Step
    {
        private stepType operation;
        private int value;
        private Dictionary<string, double> terminationCriteria = new Dictionary<string, double>();
        private stepStatus status;

        public stepType Operation
        {
            get
            {
                return operation;
            }
        }

        public int Value
        {
            get
            {
                return value;
            }
        }

        public Dictionary<string, double> TerminationCriteria
        {
            get
            {
                return terminationCriteria;
            }
        }

        public stepStatus Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }

        public Step(stepType operation, int value, string terminationCriterion, double terminationValue)
        {
            this.operation = operation;
            this.value = value;

            terminationCriteria[terminationCriterion] = terminationValue;

            status = stepStatus.Incompleted;
        }

        public Step(stepType operation, int value, string terminationCriterion1, double terminationValue1, string terminationCriterion2, double terminationValue2)
        {
            this.operation = operation;
            this.value = value;

            terminationCriteria[terminationCriterion1] = terminationValue1;
            terminationCriteria[terminationCriterion2] = terminationValue2;

            status = stepStatus.Incompleted;
        }

        public Step(stepType operation, int value, string terminationCriterion1, double terminationValue1, string terminationCriterion2, double terminationValue2, string terminationCriterion3, double terminationValue3)
        {
            this.operation = operation;
            this.value = value;

            terminationCriteria[terminationCriterion1] = terminationValue1;
            terminationCriteria[terminationCriterion2] = terminationValue2;
            terminationCriteria[terminationCriterion3] = terminationValue3;

            status = stepStatus.Incompleted;
        }
    }
}
