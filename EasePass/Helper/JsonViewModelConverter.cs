using EasePass.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace EasePass.Helper
{
    internal static class JsonViewModelConverter
    {
        public static string GetJsonFromViewModel(JsonNodeViewModel viewModel)
        {
            if (viewModel.Key != "jvmroot")
                throw new ArgumentException("The root view model must have the key 'jvmroot'.");
            if (viewModel?.Value == null)
                return null;

            return viewModel.Value.ToJsonString();
        }

        public static JsonNodeViewModel GetViewModelFromJson(string json)
        {
            JsonNode node = JsonNode.Parse(json);
            return Convert("jvmroot", node);
        }

        private static JsonNodeViewModel Convert(string key, JsonNode node)
        {
            if(node is JsonArray array)
            {
                var viewModel = new JsonNodeViewModel
                {
                    Key = key,
                    Value = array,
                    Children = new ObservableCollection<JsonNodeViewModel>()
                };
                for (int i = 0; i < array.Count; i++)
                {
                    var childNode = array[i];
                    var childViewModel = Convert(i.ToString(), childNode);
                    viewModel.Children.Add(childViewModel);
                }
                return viewModel;
            }
            if(node is JsonObject obj)
            {
                var viewModel = new JsonNodeViewModel
                {
                    Key = key,
                    Value = obj,
                    Children = new ObservableCollection<JsonNodeViewModel>()
                };
                foreach (var kvp in obj)
                {
                    var childViewModel = Convert(kvp.Key, kvp.Value);
                    viewModel.Children.Add(childViewModel);
                }
            }
            if(node is JsonValue value)
            {
                return new JsonNodeViewModel
                {
                    Key = key,
                    Value = value,
                    Children = null
                };
            }
            return new JsonNodeViewModel
            {
                Key = key,
                Value = null,
                Children = null
            };
        }
    }
}
