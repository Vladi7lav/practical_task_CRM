// JavaScript source code
function Auto_Complete_Mobilephone()
{
    var to = Xrm.Page.getAttribute("to").getValue();
    var phonenumberEl = Xrm.Page.getAttribute("new_phone_number_recipient");

    if (to != null && to.length != 0) {
        var phonenumber;
        var repeat = false;
        var Guid, mass = [];
        
        var j = 0;
        setPhoneNumber = function (recipient, callback)
        {
            phonenumber = phonenumberEl.getValue();
            if (phonenumber != null) {
                mass = phonenumber.split(/s*;s*[^$]/);
            }

            Guid = recipient.id.substr(1, 36);
            var req = new XMLHttpRequest();
            req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v8.0/" + recipient.entityType + "s(" + Guid + ")?$select=telephone1", true);
            req.setRequestHeader("OData-MaxVersion", "4.0");
            req.setRequestHeader("OData-Version", "4.0");
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
            req.onreadystatechange = function () {
                if (this.readyState === 4) {
                    req.onreadystatechange = null;
                    if (this.status === 200) {
                        var result = JSON.parse(this.response);
                        for (var i = 0; i < mass.length; i++)
                        {
                            if (result["telephone1"] == mass[i]) {
                                repeat = true;
                                break;
                            }
                            else if (i == mass.length - 1)
                                repeat = false;
                        }
                        if (!repeat) {
                            phonenumber = (phonenumber === null) ? "" : (phonenumber.search(/\s*;\s*$/)===-1 ? (phonenumber + "; "):phonenumber);
                            phonenumberEl.setValue(phonenumber + result["telephone1"] + "; ");
                        }
                        callback();
                    }
                    else {
                        Xrm.Utility.alertDialog(this.statusText);
                    }
                }
            };
            req.send();
        };
        function callback() {
            j++;
            if (j < to.length)
                setPhoneNumber(to[j], callback);
        }
        setPhoneNumber(to[0], callback);
    }
}