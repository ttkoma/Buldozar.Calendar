// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API

window.buldozar = {
  
    calendar : {
        init: function (element, netRef) {
            debugger;
            // Initialize all input of date type.
            //const calendars = window.bulmaCalendar.attach('[type="date"]', options);

            let options;
            const calendars = window.bulmaCalendar.attach(element, options);

            //// Loop on each calendar initialized
            calendars.forEach(calendar => {
                calendar.netRef = netRef;

                // Add listener to date:selected event
                calendar.on('select', ev => {
                    console.log(ev);
                    debugger;

                    ev.data.netRef.invokeMethodAsync('OnJsDateChanged', ev.data.value())
                        .then(r => console.log(r));
                });

                calendar.on('hide', ev => {
                    console.log(ev);
                    debugger;

                    ev.data.netRef.invokeMethodAsync('OnJsStateChanged', ev.data.isOpen())
                        .then(r => console.log(r));
                });

                calendar.on('show', ev => {
                    console.log(ev);
                    debugger;

                    ev.data.netRef.invokeMethodAsync('OnJsStateChanged', ev.data.isOpen())
                        .then(r => console.log(r));
                });

                calendar.onReady = function () {

                    this.netRef.invokeMethodAsync('OnJsStateChanged', ev.data.isOpen())
                        .then(r => console.log(r));
                };

                calendar.on('clear', ev => {
                    console.log(ev);

                    ev.data.netRef.invokeMethodAsync('OnJsClear')
                        .then(r => console.log(r));
                });
            });
        },

        get: function (element) {
            // To access to bulmaCalendar instance of an element
            if (element) {
                element.bulmaCalendar.on('select', datepicker => {
                    console.log(datepicker.data.value());
                });

                return element.bulmaCalendar;
            }
        },

        show: function (element) {
            if (element) {
                element.bulmaCalendar.show();
            }
        },

        hide: function (element) {
            if (element) {
                element.bulmaCalendar.hide();
            }
        },

        isOpen: function (element) {
            if (element) {
                return element.bulmaCalendar.isOpen();
            }
        },

        clear: function (element) {
            if (element) {
                element.bulmaCalendar.clear();
            }
        },

        isRange: function (element) {
            if (element) {
                return element.bulmaCalendar.isRange();
            }
        },

        save: function (element) {
            if (element) {
                element.bulmaCalendar.save();
            }
        },

        refresh: function (element) {
            if (element) {
                element.bulmaCalendar.refresh();
            }
        },

        getValue: function (element) {
            if (element) {
                return element.bulmaCalendar.value();
            }
        },

        getStartRangeValue: function (element) {
            if (element) {
                return element.bulmaCalendar.value().startDate;
            }
        },

        getEndRangeValue: function (element) {
            if (element) {
                return element.bulmaCalendar.value().endDate;
            }
        },

        setValue: function (element, datestring) {
            if (element) {
                element.bulmaCalendar.value(datestring);
            }
        },

        returnArrayAsyncJs: function (dotnetRef) {
            return dotnetRef.invokeMethodAsync('ReturnNowDateAsync')
                .then(r => console.log(r));
        },

    }

};
