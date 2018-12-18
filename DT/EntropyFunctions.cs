using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data;
using System.Data.SqlClient;

namespace DT
{
    public static class EntropyFunctions
    {
        public static List<double> class_probabilities(List<string> labels)//результаты - сюда передаем столбец results (true/false)
        {
            int countTrue = labels.Count(x => x =="true");
            int countFalse = labels.Count(x => x == "false");
            int countLabels = labels.Count;

            List<double> classProbabilities = new List<double>();
            classProbabilities.Add((double)countTrue / countLabels);
            classProbabilities.Add((double)countFalse / countLabels);

            return classProbabilities;
        }

        public static double entropy(List<double> classProbabilities)
        {
            double entropy = 0;
            foreach (double probability in classProbabilities)
            {
                if (probability > 0)
                    entropy += (-probability) * Math.Log(probability, 2);
            }
            return entropy;
        }
        
        public static double data_entropy(DataTable inputs)
        {
            List<string> results = new List<string>();
            foreach(DataRow dataRow in inputs.Rows)
            {
                results.Add(Convert.ToString(dataRow["Result"]));
            }
            List<double> probabilities = class_probabilities(results);
            double _entropy = entropy(probabilities);
            return _entropy;
        }
            
        public static DataTable partition_by(DataTable inputs, string attribute)
        {
            DataTable partitionTable = new DataTable();
            partitionTable = inputs.Copy();
            partitionTable.Columns[attribute].SetOrdinal(0);
            return partitionTable;
        }

        static double partition_entropy(DataTable partition)
        {
            int total_count = partition.Rows.Count;
            double partition_entropy = 0;
            
            string partitionColumnName = partition.Columns[0].ColumnName;
            List<string> partitionAttributeValues = new List<string>();
            foreach(DataRow dataRow in partition.Rows)
            {
                partitionAttributeValues.Add(Convert.ToString(dataRow[0]));
            }
            IEnumerable<string> uniquePartitionAttributeValues = partitionAttributeValues.Distinct();

            foreach (string value in uniquePartitionAttributeValues)
            {
                IEnumerable<DataRow> query =
                            from part in partition.AsEnumerable()
                            where part.Field<String>(partitionColumnName) == value
                            select part;
                DataTable subset_inputs = query.CopyToDataTable<DataRow>();
                double count_subset_inputs = Convert.ToDouble(subset_inputs.Rows.Count);
                partition_entropy += (double)count_subset_inputs / total_count * data_entropy(subset_inputs);
            }
            return partition_entropy;
        }
        
        public static string SelectBestAttributeToSplit(DataTable inputs)
        {
            List<string> columnNames = new List<string>();
            foreach(DataColumn dataColumn in inputs.Columns)
            {
                if(dataColumn.ColumnName != "Result")
                {
                    columnNames.Add(dataColumn.ColumnName);
                }
            }
            Dictionary<string, double> attributeEntropyPairs = new Dictionary<string, double>();
            foreach(string name in columnNames)
            {
                DataTable partitionTable = new DataTable();
                partitionTable = partition_by(inputs, name);
                double partitionEntropy = partition_entropy(partitionTable);
                attributeEntropyPairs.Add(name, partitionEntropy);
            }
            var bestPair = attributeEntropyPairs.OrderBy(k => k.Value).FirstOrDefault();

            return bestPair.Key;
        }
        
        public static Attribute SelectBestAttributeToSplit(List<Attribute> attributes, DataTable inputs)
        {
            List<string> attributeNames = new List<string>();
            foreach(Attribute attribute in attributes)
            {
                attributeNames.Add(attribute.Name);
            }
            Dictionary<string, double> attributeEntropyPairs = new Dictionary<string, double>();
            foreach (string name in attributeNames)
            {
                DataTable partitionTable = new DataTable();
                partitionTable = partition_by(inputs, name);
                double partitionEntropy = partition_entropy(partitionTable);
                attributeEntropyPairs.Add(name, partitionEntropy);
            }
            var bestPair = attributeEntropyPairs.OrderBy(k => k.Value).FirstOrDefault();
            return attributes.FirstOrDefault(e => e.Name == bestPair.Key);
        }

        public static List<string> GetAttributeValues(DataTable inputs, string attribute)
        {
            List<string> attributeValues = new List<string>();
            foreach (DataRow dataRow in inputs.Rows)
            {
                attributeValues.Add(Convert.ToString(dataRow[attribute]));
            }
            attributeValues = attributeValues.Distinct().ToList();
            return attributeValues;
        }

        public static int CountAttributeValues(DataTable inputs, string attribute)
        {
            List<string> attributeValues = new List<string>();
            attributeValues = GetAttributeValues(inputs, attribute);
            return attributeValues.Count;
        }

        public static List<string> GetAllResults(DataTable inputs)
        {
            List<string> results = new List<string>();
            foreach (DataRow dataRow in inputs.Rows)
            {
                results.Add(Convert.ToString(dataRow[inputs.Columns[inputs.Columns.Count - 1]]));
            }
            return results;
        }


        public static List<string> GetAllDifferentResults(DataTable inputs)
        {
            List<string> results = GetAllResults(inputs);
            results = results.Distinct().ToList();
            return results;
        }

        public static bool AllResultsAreTrue(DataTable inputs)
        {
            List<string> results = GetAllDifferentResults(inputs);
            if (results.Count == 1 && results.Contains("true"))
                return true;
            return false;

        }
        public static bool AllResultsAreFalse(DataTable inputs)
        {
            List<string> results = GetAllDifferentResults(inputs);
            if (results.Count == 1 && results.Contains("false"))
                return true;
            return false;

        }

        public static string MostCommonResult(DataTable inputs)
        {
            int countTrue = 0;
            int countFalse = 0;
            string mostCommonResult = "";
            List<string> results = GetAllResults(inputs);
            foreach (string result in results)
            {
                if (result == "true")
                    countTrue++;
                if (result == "false")
                    countFalse++;
            }
            if (countTrue >= countFalse)
                mostCommonResult = "true";
            else
                mostCommonResult = "false";
            return mostCommonResult;
        }

        public static DataTable  SelectDataByAttributeAndValues(Attribute attribute, DataTable inputs, string attributeValue)
        {
            DataTable subset = new DataTable();
            subset = inputs.Clone();
            foreach(DataRow dataRow in inputs.Rows)
            {
                if(dataRow[attribute.Name].ToString() == attributeValue)
                {
                    subset.ImportRow(dataRow);
                }
            }
            return subset;
        }        
    }
}
