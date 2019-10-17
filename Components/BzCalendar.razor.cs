using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Buldozar.Components.Calendar
{
    public class BzCalendarBase : ComponentBase
    {
        [Inject]
        IJSRuntime _jsRuntime { get; set; }

        protected ElementReference _component;
        private bool isInitialized;

        [Parameter]
        public CalendarDisplayMode DisplayMode
        {
            get
            {
                string raw = GetAttribute("data-display-mode");

                switch (raw as string)
                {
                    case "inline":
                        return CalendarDisplayMode.Inline;
                    case "dialog":
                        return CalendarDisplayMode.Dialog;
                    default:
                        return CalendarDisplayMode.Default;
                }
            }

            set
            {
                InputAttributes["data-display-mode"] = value.ToString().ToLower();
            }
        }

        [Parameter]
        public string Language
        {
            get
            {
                return GetAttribute("data-lang");
            }
            set
            {
                InputAttributes["data-lang"] = value;
            }
        }

        [Parameter]
        public string Format
        {
            get
            {
                return GetAttribute("data-date-format");
            }
            set
            {
                InputAttributes["data-date-format"] = value;
            }
        }

        [Parameter]
        public Dictionary<string, object> InputAttributes { get; set; } =
            new Dictionary<string, object>()
            {
            { "data-color", "grey" },
            { "data-today-label", "Сегодня" },
            { "data-show-clear-button", "false" },
            { "data-show-buttons", false },
            { "data-clear-label", "Очистить" }
            };

        [Parameter]
        public DateTime Value { get; set; } = DateTime.Now;

        [Parameter]
        public EventCallback<ChangeEventArgs> OnValueChanged { get; set; }


        public bool IsOpen { get; set; }

        protected override void OnInitialized()
        {
            if (string.IsNullOrEmpty(Language))
                Language = "ru";

            if (string.IsNullOrEmpty(Format))
                Format = "DD.MM.YYYY";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitAsync();
                
                if(DisplayMode == CalendarDisplayMode.Inline)
                    await HideAsync();
            }
        }

        private string GetAttribute(string key)
        {
            if (InputAttributes.ContainsKey(key))
                return InputAttributes[key].ToString();

            return null;
        }

        #region JsInterop
        
        /// <summary>
        /// Js Initialize Bulma calendar cimponent
        /// </summary>
        /// <returns></returns>
        public ValueTask InitAsync()
        {
            isInitialized = true;
            var netRef = DotNetObjectReference.Create(this);
            return _jsRuntime.InvokeVoidAsync("buldozar.calendar.init", _component, netRef);
        }

        /// <summary>
        /// Open date picker(not available with "inline" display style)
        /// </summary>
        public ValueTask ShowAsync()
        {
            return _jsRuntime.InvokeVoidAsync("buldozar.calendar.show", _component);
        }

        /// <summary>
        /// Close date picker (not available with "inline" display style)
        /// </summary>
        /// <returns></returns>
        public ValueTask HideAsync()
        {
            return _jsRuntime.InvokeVoidAsync("buldozar.calendar.hide", _component);
        }

        /// <summary>
        /// Check if date picker is open or not
        /// </summary>
        /// <returns>Type: bool - True if date picker is open else False</returns>
        public ValueTask<bool> IsOpenAsync()
        {
            if (!isInitialized)
                return new ValueTask<bool>(false);
            
            return _jsRuntime.InvokeAsync<bool>("buldozar.calendar.isOpen", _component);
        }

        /// <summary>
        /// Check if current instance is a range date picker
        /// </summary>
        /// <returns>Type: bool - True if the instance is a range date picker</returns>
        public ValueTask<bool> IsRangeAsync()
        {
            return _jsRuntime.InvokeAsync<bool>("buldozar.calendar.isRange", _component);
        }

        /// <summary>
        /// Get the date picker value
        /// </summary>
        /// <returns>Type: DateTime - Date value</returns>
        public ValueTask<string> GetValueAsync()
        {
            //var str = _jsRuntime.InvokeAsync<object>("bulma.calendar.getValue", _component).Result;
            return _jsRuntime.InvokeAsync<string>("buldozar.calendar.getValue", _component);
        }

        /// <summary>
        /// Force calendar refresh
        /// </summary>
        /// <param name="component"></param>
        public ValueTask RefreshAsync()
        {
            return _jsRuntime.InvokeVoidAsync("buldozar.calendar.refresh", _component);
        }

        /// <summary>
        /// Force to set calendar data into UI inputs
        /// </summary>
        public ValueTask SaveAsync()
        {
            return _jsRuntime.InvokeVoidAsync("buldozar.calendar.save", _component);
        }

        /// <summary>
        /// Force to set calendar data into UI inputs
        /// </summary>
        public ValueTask ClearAsync()
        {
            return _jsRuntime.InvokeVoidAsync("buldozar.calendar.clear", _component);
        }

        [JSInvokable]
        public async Task OnJsDateChanged(string dateInFormat)
        {

            var tmp = DateTime.Parse(dateInFormat);
            if (!Value.Equals(tmp))
            {
                Value = tmp;
                StateHasChanged();

                if (OnValueChanged.HasDelegate)
                {
                    await OnValueChanged.InvokeAsync(new ChangeEventArgs() { Value = Value });
                }
            }
        }

        [JSInvokable]
        public async Task OnJsClear()
        {
            Value = DateTime.MinValue;
            StateHasChanged();
            if (OnValueChanged.HasDelegate)
            {
                await OnValueChanged.InvokeAsync(new ChangeEventArgs() { Value = Value });
            }
        }

        [JSInvokable]
        public async Task OnJsStateChanged(bool state)
        {
            if (!IsOpen.Equals(state))
            {
                IsOpen = state;
                StateHasChanged();
            }

            bool open = await IsOpenAsync();
        }

        #endregion

    }

    public enum CalendarDisplayMode
    {
        Default,
        Dialog,
        Inline
    }
}
