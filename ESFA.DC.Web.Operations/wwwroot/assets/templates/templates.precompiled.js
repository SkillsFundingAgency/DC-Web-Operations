(function() {
  var template = Handlebars.template, templates = Handlebars.templates = Handlebars.templates || {};
templates['PeriodEnd/ALLFPeriodEndFileList'] = template({"1":function(container,depth0,helpers,partials,data) {
    var helper, alias1=container.lambda, alias2=container.escapeExpression, alias3=depth0 != null ? depth0 : (container.nullContext || {}), alias4=container.hooks.helperMissing, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "            <tr class=\"govuk-table__row\">\r\n                <td class=\"govuk-table__cell\">\r\n                        <a href=\"/periodendallf/periodEnd/getReportFile/"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"fileName") : depth0), depth0))
    + "\">"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"fileName") : depth0), depth0))
    + "</a>\r\n                </td>\r\n                <td class=\"govuk-table__cell\">\r\n                    <span class=\"$"
    + alias2((lookupProperty(helpers,"jobStatusClass")||(depth0 && lookupProperty(depth0,"jobStatusClass"))||alias4).call(alias3,(depth0 != null ? lookupProperty(depth0,"displayStatus") : depth0),{"name":"jobStatusClass","hash":{},"data":data,"loc":{"start":{"line":16,"column":34},"end":{"line":16,"column":71}}}))
    + "\">"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"displayStatus") : depth0), depth0))
    + "</span> <br />\r\n                    <span class=\"govuk-!-font-weight-bold\">"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"recordCount") : depth0), depth0))
    + " records</span> <br />\r\n                    <span class=\"govuk-!-font-weight-bold\">"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"errorCount") : depth0), depth0))
    + " errors</span> <br />\r\n                </td>\r\n                <td class=\"govuk-table__cell\"><a href=\"/periodendallf/periodEnd/getReportFile/"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"reportName") : depth0), depth0))
    + "/"
    + alias2(((helper = (helper = lookupProperty(helpers,"period") || (depth0 != null ? lookupProperty(depth0,"period") : depth0)) != null ? helper : alias4),(typeof helper === "function" ? helper.call(alias3,{"name":"period","hash":{},"data":data,"loc":{"start":{"line":20,"column":114},"end":{"line":20,"column":124}}}) : helper)))
    + "/"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"jobId") : depth0), depth0))
    + "\">"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"reportName") : depth0), depth0))
    + "</a>\r\n                </td>\r\n            </tr>\r\n";
},"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "﻿<table class=\"govuk-table\">\r\n    <thead class=\"govuk-table__head\">\r\n        <tr class=\"govuk-table__row\">\r\n            <th scope=\"col\" class=\"govuk-table__header\">File</th>\r\n            <th scope=\"col\" class=\"govuk-table__header table-width-min\">Status</th>\r\n            <th scope=\"col\" class=\"govuk-table__header\">Reports</th>\r\n        </tr>\r\n    </thead>\r\n    <tbody class=\"govuk-table__body\">\r\n"
    + ((stack1 = lookupProperty(helpers,"each").call(depth0 != null ? depth0 : (container.nullContext || {}),(depth0 != null ? lookupProperty(depth0,"files") : depth0),{"name":"each","hash":{},"fn":container.program(1, data, 0),"inverse":container.noop,"data":data,"loc":{"start":{"line":10,"column":8},"end":{"line":23,"column":17}}})) != null ? stack1 : "")
    + "    </tbody>\r\n</table>";
},"useData":true});
templates['PeriodEnd/PeriodEnd'] = template({"1":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "    <li>\r\n"
    + ((stack1 = container.invokePartial(lookupProperty(partials,"pathHeader"),depth0,{"name":"pathHeader","hash":{"parentPathName":(lookupProperty(helpers,"getPathNameBySubPathId")||(depth0 && lookupProperty(depth0,"getPathNameBySubPathId"))||alias2).call(alias1,(depths[1] != null ? lookupProperty(depths[1],"viewModel") : depths[1]),(depth0 != null ? lookupProperty(depth0,"pathId") : depth0),{"name":"getPathNameBySubPathId","hash":{},"data":data,"loc":{"start":{"line":4,"column":48},"end":{"line":4,"column":97}}}),"path":depth0},"data":data,"indent":"        ","helpers":helpers,"partials":partials,"decorators":container.decorators})) != null ? stack1 : "")
    + "        <ul class=\"app-task-list__items\" id=\"PI-"
    + container.escapeExpression((lookupProperty(helpers,"removeSpaces")||(depth0 && lookupProperty(depth0,"removeSpaces"))||alias2).call(alias1,(depth0 != null ? lookupProperty(depth0,"name") : depth0),{"name":"removeSpaces","hash":{},"data":data,"loc":{"start":{"line":5,"column":48},"end":{"line":5,"column":74}}}))
    + "\">\r\n"
    + ((stack1 = lookupProperty(helpers,"each").call(alias1,(depth0 != null ? lookupProperty(depth0,"pathItems") : depth0),{"name":"each","hash":{},"fn":container.program(2, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":6,"column":12},"end":{"line":36,"column":21}}})) != null ? stack1 : "")
    + "        </ul>\r\n    </li>\r\n";
},"2":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, alias3=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "            "
    + alias3((lookupProperty(helpers,"setVar")||(depth0 && lookupProperty(depth0,"setVar"))||alias2).call(alias1,"isCurrent",(lookupProperty(helpers,"isPathItemCurrent")||(depth0 && lookupProperty(depth0,"isPathItemCurrent"))||alias2).call(alias1,(depth0 != null ? lookupProperty(depth0,"ordinal") : depth0),(depths[1] != null ? lookupProperty(depths[1],"position") : depths[1]),{"name":"isPathItemCurrent","hash":{},"data":data,"loc":{"start":{"line":7,"column":33},"end":{"line":7,"column":77}}}),{"name":"setVar","hash":{},"data":data,"loc":{"start":{"line":7,"column":12},"end":{"line":7,"column":79}}}))
    + "\r\n            "
    + alias3((lookupProperty(helpers,"setVar")||(depth0 && lookupProperty(depth0,"setVar"))||alias2).call(alias1,"isAutoCompleted",(lookupProperty(helpers,"isAutoCompleted")||(depth0 && lookupProperty(depth0,"isAutoCompleted"))||alias2).call(alias1,depth0,depths[1],{"name":"isAutoCompleted","hash":{},"data":data,"loc":{"start":{"line":8,"column":39},"end":{"line":8,"column":64}}}),{"name":"setVar","hash":{},"data":data,"loc":{"start":{"line":8,"column":12},"end":{"line":8,"column":66}}}))
    + "\r\n\r\n"
    + ((stack1 = container.invokePartial(lookupProperty(partials,"proceedableItemWrapper"),depth0,{"name":"proceedableItemWrapper","hash":{"isBusy":(depths[1] != null ? lookupProperty(depths[1],"isBusy") : depths[1]),"isNextItemSubPath":(lookupProperty(helpers,"isNextItemSubPath")||(depth0 && lookupProperty(depth0,"isNextItemSubPath"))||alias2).call(alias1,(depths[1] != null ? lookupProperty(depths[1],"pathItems") : depths[1]),(data && lookupProperty(data,"index")),{"name":"isNextItemSubPath","hash":{},"data":data,"loc":{"start":{"line":10,"column":178},"end":{"line":10,"column":217}}}),"yearPeriod":((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"yearPeriod")),"isProceedable":(lookupProperty(helpers,"isProceedable")||(depth0 && lookupProperty(depth0,"isProceedable"))||alias2).call(alias1,((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"isCurrent")),((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"isAutoCompleted")),(data && lookupProperty(data,"last")),{"name":"isProceedable","hash":{},"data":data,"loc":{"start":{"line":10,"column":72},"end":{"line":10,"column":131}}}),"pathitem":depth0},"fn":container.program(3, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"helpers":helpers,"partials":partials,"decorators":container.decorators})) != null ? stack1 : "");
},"3":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, alias3=container.escapeExpression, alias4=container.lambda, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                <li id=\""
    + alias3((lookupProperty(helpers,"removeSpaces")||(depth0 && lookupProperty(depth0,"removeSpaces"))||alias2).call(alias1,(depth0 != null ? lookupProperty(depth0,"name") : depth0),{"name":"removeSpaces","hash":{},"data":data,"loc":{"start":{"line":11,"column":24},"end":{"line":11,"column":50}}}))
    + "\" class=\"app-task-list__item\">\r\n                    "
    + ((stack1 = lookupProperty(helpers,"if").call(alias1,((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"isCurrent")),{"name":"if","hash":{},"fn":container.program(4, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":12,"column":20},"end":{"line":12,"column":53}}})) != null ? stack1 : "")
    + alias3(alias4((depth0 != null ? lookupProperty(depth0,"name") : depth0), depth0))
    + ((stack1 = lookupProperty(helpers,"if").call(alias1,((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"isAutoCompleted")),{"name":"if","hash":{},"fn":container.program(6, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":12,"column":66},"end":{"line":12,"column":124}}})) != null ? stack1 : "")
    + ((stack1 = lookupProperty(helpers,"if").call(alias1,((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"isCurrent")),{"name":"if","hash":{},"fn":container.program(8, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":12,"column":124},"end":{"line":12,"column":158}}})) != null ? stack1 : "")
    + "\r\n\r\n"
    + ((stack1 = lookupProperty(helpers,"if").call(alias1,(lookupProperty(helpers,"isSubPath")||(depth0 && lookupProperty(depth0,"isSubPath"))||alias2).call(alias1,depth0,{"name":"isSubPath","hash":{},"data":data,"loc":{"start":{"line":14,"column":22},"end":{"line":14,"column":38}}}),{"name":"if","hash":{},"fn":container.program(10, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":14,"column":16},"end":{"line":16,"column":23}}})) != null ? stack1 : "")
    + "\r\n                <a id=\""
    + alias3(alias4((depth0 != null ? lookupProperty(depth0,"name") : depth0), depth0))
    + "\"></a>\r\n"
    + ((stack1 = lookupProperty(helpers,"if").call(alias1,(depth0 != null ? lookupProperty(depth0,"pathItemJobs") : depth0),{"name":"if","hash":{},"fn":container.program(12, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":19,"column":16},"end":{"line":33,"column":23}}})) != null ? stack1 : "")
    + "            </li>\r\n";
},"4":function(container,depth0,helpers,partials,data) {
    return "<b>";
},"6":function(container,depth0,helpers,partials,data) {
    return " - Status : Completed";
},"8":function(container,depth0,helpers,partials,data) {
    return "</b>";
},"10":function(container,depth0,helpers,partials,data) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                    <a href=\"#Path"
    + container.escapeExpression(container.lambda(((stack1 = (depth0 != null ? lookupProperty(depth0,"subPaths") : depth0)) != null ? lookupProperty(stack1,"0") : stack1), depth0))
    + "\">❯</a>\r\n";
},"12":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, alias1=depth0 != null ? depth0 : (container.nullContext || {}), lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                <ul id=\"JL-"
    + container.escapeExpression((lookupProperty(helpers,"removeSpaces")||(depth0 && lookupProperty(depth0,"removeSpaces"))||container.hooks.helperMissing).call(alias1,(depth0 != null ? lookupProperty(depth0,"name") : depth0),{"name":"removeSpaces","hash":{},"data":data,"loc":{"start":{"line":20,"column":27},"end":{"line":20,"column":53}}}))
    + "\">\r\n"
    + ((stack1 = lookupProperty(helpers,"each").call(alias1,(depth0 != null ? lookupProperty(depth0,"pathItemJobs") : depth0),{"name":"each","hash":{},"fn":container.program(13, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":21,"column":20},"end":{"line":31,"column":29}}})) != null ? stack1 : "")
    + "                </ul>\r\n";
},"13":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, alias1=container.escapeExpression, alias2=depth0 != null ? depth0 : (container.nullContext || {}), alias3=container.hooks.helperMissing, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                    <li>\r\n                        Job Id : "
    + alias1(container.lambda((depth0 != null ? lookupProperty(depth0,"jobId") : depth0), depth0))
    + ", "
    + alias1((lookupProperty(helpers,"getProviderName")||(depth0 && lookupProperty(depth0,"getProviderName"))||alias3).call(alias2,(depth0 != null ? lookupProperty(depth0,"providerName") : depth0),{"name":"getProviderName","hash":{},"data":data,"loc":{"start":{"line":23,"column":49},"end":{"line":23,"column":86}}}))
    + "Status : "
    + alias1((lookupProperty(helpers,"getStatus")||(depth0 && lookupProperty(depth0,"getStatus"))||alias3).call(alias2,(depth0 != null ? lookupProperty(depth0,"status") : depth0),{"name":"getStatus","hash":{},"data":data,"loc":{"start":{"line":23,"column":95},"end":{"line":23,"column":120}}}))
    + "\r\n"
    + ((stack1 = lookupProperty(helpers,"if").call(alias2,(lookupProperty(helpers,"canRetry")||(depth0 && lookupProperty(depth0,"canRetry"))||alias3).call(alias2,(depth0 != null ? lookupProperty(depth0,"status") : depth0),{"name":"canRetry","hash":{},"data":data,"loc":{"start":{"line":24,"column":30},"end":{"line":24,"column":52}}}),{"name":"if","hash":{},"fn":container.program(14, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":24,"column":24},"end":{"line":26,"column":31}}})) != null ? stack1 : "")
    + "                    </li>\r\n"
    + ((stack1 = lookupProperty(helpers,"if").call(alias2,(data && lookupProperty(data,"last")),{"name":"if","hash":{},"fn":container.program(16, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":28,"column":20},"end":{"line":30,"column":27}}})) != null ? stack1 : "");
},"14":function(container,depth0,helpers,partials,data) {
    var alias1=container.lambda, alias2=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                        <a href=\"#/\" class=\"govuk-link govuk-!-margin-left-3\" id=\"retryJob_"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"jobId") : depth0), depth0))
    + "\" onclick=\"window.periodEndClient.resubmitJob("
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"jobId") : depth0), depth0))
    + ")\">Retry</a>\r\n";
},"16":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return ((stack1 = container.invokePartial(lookupProperty(partials,"pathItemJobSummary"),depth0,{"name":"pathItemJobSummary","hash":{"summary":(depths[1] != null ? lookupProperty(depths[1],"pathItemJobSummary") : depths[1])},"data":data,"indent":"                    ","helpers":helpers,"partials":partials,"decorators":container.decorators})) != null ? stack1 : "");
},"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "﻿<ol class=\"app-task-list\">\r\n"
    + ((stack1 = lookupProperty(helpers,"each").call(depth0 != null ? depth0 : (container.nullContext || {}),((stack1 = (depth0 != null ? lookupProperty(depth0,"viewModel") : depth0)) != null ? lookupProperty(stack1,"paths") : stack1),{"name":"each","hash":{},"fn":container.program(1, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":2,"column":4},"end":{"line":39,"column":13}}})) != null ? stack1 : "")
    + "</ol>";
},"usePartial":true,"useData":true,"useDepths":true});
templates['PeriodEnd/PeriodEndNavigation'] = template({"1":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, alias1=container.lambda, alias2=container.escapeExpression, alias3=depth0 != null ? depth0 : (container.nullContext || {}), lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "    <a class=\"govuk-heading-s\" href=\"#Path"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"pathId") : depth0), depth0))
    + "\">"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"name") : depth0), depth0))
    + "</a>\r\n    <span class=\"nav\">\r\n        <ul id=\"ST-"
    + alias2((lookupProperty(helpers,"removeSpaces")||(depth0 && lookupProperty(depth0,"removeSpaces"))||container.hooks.helperMissing).call(alias3,(depth0 != null ? lookupProperty(depth0,"name") : depth0),{"name":"removeSpaces","hash":{},"data":data,"loc":{"start":{"line":4,"column":19},"end":{"line":4,"column":45}}}))
    + "\" class=\"govuk-list\">\r\n"
    + ((stack1 = lookupProperty(helpers,"each").call(alias3,(depth0 != null ? lookupProperty(depth0,"pathItems") : depth0),{"name":"each","hash":{},"fn":container.program(2, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":5,"column":12},"end":{"line":14,"column":21}}})) != null ? stack1 : "")
    + "        </ul>\r\n    </span>\r\n";
},"2":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, alias1=depth0 != null ? depth0 : (container.nullContext || {}), lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return ((stack1 = lookupProperty(helpers,"if").call(alias1,(lookupProperty(helpers,"includeInNav")||(depth0 && lookupProperty(depth0,"includeInNav"))||container.hooks.helperMissing).call(alias1,depth0,depths[1],{"name":"includeInNav","hash":{},"data":data,"loc":{"start":{"line":6,"column":22},"end":{"line":6,"column":44}}}),{"name":"if","hash":{},"fn":container.program(3, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":6,"column":16},"end":{"line":13,"column":23}}})) != null ? stack1 : "");
},"3":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, alias3=container.lambda, alias4=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                    <li class=\"compact-list-item\">\r\n"
    + ((stack1 = lookupProperty(helpers,"if").call(alias1,(lookupProperty(helpers,"isPathItemCurrent")||(depth0 && lookupProperty(depth0,"isPathItemCurrent"))||alias2).call(alias1,(depth0 != null ? lookupProperty(depth0,"ordinal") : depth0),(depths[1] != null ? lookupProperty(depths[1],"position") : depths[1]),{"name":"isPathItemCurrent","hash":{},"data":data,"loc":{"start":{"line":8,"column":30},"end":{"line":8,"column":74}}}),{"name":"if","hash":{},"fn":container.program(4, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":8,"column":24},"end":{"line":10,"column":31}}})) != null ? stack1 : "")
    + "                        <a class=\"govuk-link small-link "
    + ((stack1 = lookupProperty(helpers,"if").call(alias1,(lookupProperty(helpers,"isPathItemCurrent")||(depth0 && lookupProperty(depth0,"isPathItemCurrent"))||alias2).call(alias1,(depth0 != null ? lookupProperty(depth0,"ordinal") : depth0),(depths[1] != null ? lookupProperty(depths[1],"position") : depths[1]),{"name":"isPathItemCurrent","hash":{},"data":data,"loc":{"start":{"line":11,"column":62},"end":{"line":11,"column":106}}}),{"name":"if","hash":{},"fn":container.program(6, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":11,"column":56},"end":{"line":11,"column":121}}})) != null ? stack1 : "")
    + "\" href=\"#"
    + alias4(alias3((depth0 != null ? lookupProperty(depth0,"name") : depth0), depth0))
    + "\">"
    + alias4(alias3((depth0 != null ? lookupProperty(depth0,"name") : depth0), depth0))
    + "</a>\r\n                    </li>\r\n";
},"4":function(container,depth0,helpers,partials,data) {
    return "                            <div class=\"triangle-right\"></div>\r\n";
},"6":function(container,depth0,helpers,partials,data) {
    return "active";
},"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "﻿"
    + ((stack1 = lookupProperty(helpers,"each").call(depth0 != null ? depth0 : (container.nullContext || {}),(depth0 != null ? lookupProperty(depth0,"paths") : depth0),{"name":"each","hash":{},"fn":container.program(1, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":1,"column":1},"end":{"line":17,"column":9}}})) != null ? stack1 : "");
},"useData":true,"useDepths":true});
templates['PeriodEnd/ValidityPeriods'] = template({"1":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, alias1=container.lambda, alias2=container.escapeExpression, alias3=depth0 != null ? depth0 : (container.nullContext || {}), alias4=container.hooks.helperMissing, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "    <li>\r\n        <input type=\"hidden\" class=\"hidden\"\r\n               id=\"path-"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"pathId") : depth0), depth0))
    + "\"\r\n               name=\"items["
    + alias2(alias1(((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"masterIndex")), depth0))
    + "].Id\"\r\n               value=\""
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"pathId") : depth0), depth0))
    + "\" />\r\n\r\n        <input type=\"hidden\" class=\"hidden\"\r\n               id=\"type-"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"pathId") : depth0), depth0))
    + "\"\r\n               name=\"items["
    + alias2(alias1(((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"masterIndex")), depth0))
    + "].EntityType\"\r\n               value=\""
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"entityType") : depth0), depth0))
    + "\" />\r\n\r\n\r\n        <div class=\"checkbox-container\">\r\n            <div class=\"checkbox-text govuk-!-font-weight-bold\">\r\n"
    + ((stack1 = lookupProperty(helpers,"if").call(alias3,(depth0 != null ? lookupProperty(depth0,"isCritical") : depth0),{"name":"if","hash":{},"fn":container.program(2, data, 0, blockParams, depths),"inverse":container.program(4, data, 0, blockParams, depths),"data":data,"loc":{"start":{"line":21,"column":16},"end":{"line":25,"column":23}}})) != null ? stack1 : "")
    + "            </div>\r\n            <div class=\"govuk-checkboxes\">\r\n                <div class=\"flex\">\r\n                    <div class=\"govuk-checkboxes__item\">\r\n"
    + ((stack1 = lookupProperty(helpers,"unless").call(alias3,(depth0 != null ? lookupProperty(depth0,"isCritical") : depth0),{"name":"unless","hash":{},"fn":container.program(6, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":30,"column":24},"end":{"line":39,"column":35}}})) != null ? stack1 : "")
    + "                    </div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n\r\n        <ul class=\"inner-list "
    + ((stack1 = lookupProperty(helpers,"if").call(alias3,(lookupProperty(helpers,"disableCheckboxes")||(depth0 && lookupProperty(depth0,"disableCheckboxes"))||alias4).call(alias3,(depth0 != null ? lookupProperty(depth0,"isValidForPeriod") : depth0),(depth0 != null ? lookupProperty(depth0,"isCritical") : depth0),{"name":"disableCheckboxes","hash":{},"data":data,"loc":{"start":{"line":45,"column":36},"end":{"line":45,"column":93}}}),{"name":"if","hash":{},"fn":container.program(11, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":45,"column":30},"end":{"line":45,"column":113}}})) != null ? stack1 : "")
    + "\">\r\n"
    + ((stack1 = lookupProperty(helpers,"each").call(alias3,(depth0 != null ? lookupProperty(depth0,"pathItems") : depth0),{"name":"each","hash":{},"fn":container.program(13, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":46,"column":12},"end":{"line":92,"column":21}}})) != null ? stack1 : "")
    + "        </ul>\r\n\r\n        <hr class=\"hr-bottom-margin\" />\r\n    </li>\r\n    "
    + alias2((lookupProperty(helpers,"setVar")||(depth0 && lookupProperty(depth0,"setVar"))||alias4).call(alias3,"masterIndex",(lookupProperty(helpers,"increment")||(depth0 && lookupProperty(depth0,"increment"))||alias4).call(alias3,((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"masterIndex")),{"name":"increment","hash":{},"data":data,"loc":{"start":{"line":97,"column":27},"end":{"line":97,"column":56}}}),{"name":"setVar","hash":{},"data":data,"loc":{"start":{"line":97,"column":4},"end":{"line":97,"column":58}}}))
    + "\r\n";
},"2":function(container,depth0,helpers,partials,data) {
    var lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                "
    + container.escapeExpression(container.lambda((depth0 != null ? lookupProperty(depth0,"name") : depth0), depth0))
    + "\r\n";
},"4":function(container,depth0,helpers,partials,data) {
    var alias1=container.lambda, alias2=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                <a href=\"#item_"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"pathId") : depth0), depth0))
    + "\" id=\""
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"pathId") : depth0), depth0))
    + "\">❮</a> "
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"name") : depth0), depth0))
    + "\r\n";
},"6":function(container,depth0,helpers,partials,data) {
    var stack1, alias1=container.lambda, alias2=container.escapeExpression, alias3=depth0 != null ? depth0 : (container.nullContext || {}), alias4=container.hooks.helperMissing, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                        <input type=\"checkbox\" class=\"govuk-checkboxes__input validityCheckbox\"\r\n                               name=\"items["
    + alias2(alias1(((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"masterIndex")), depth0))
    + "].Enabled\"\r\n                               id=\"ckpath-"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"pathId") : depth0), depth0))
    + "\"\r\n                               "
    + ((stack1 = lookupProperty(helpers,"if").call(alias3,(depth0 != null ? lookupProperty(depth0,"isPreviousPeriod") : depth0),{"name":"if","hash":{},"fn":container.program(7, data, 0),"inverse":container.noop,"data":data,"loc":{"start":{"line":34,"column":31},"end":{"line":34,"column":76}}})) != null ? stack1 : "")
    + "\r\n                               "
    + ((stack1 = lookupProperty(helpers,"if").call(alias3,(lookupProperty(helpers,"mapValidStateToBoolean")||(depth0 && lookupProperty(depth0,"mapValidStateToBoolean"))||alias4).call(alias3,(depth0 != null ? lookupProperty(depth0,"isValidForPeriod") : depth0),{"name":"mapValidStateToBoolean","hash":{},"data":data,"loc":{"start":{"line":35,"column":37},"end":{"line":35,"column":83}}}),{"name":"if","hash":{},"fn":container.program(9, data, 0),"inverse":container.noop,"data":data,"loc":{"start":{"line":35,"column":31},"end":{"line":35,"column":100}}})) != null ? stack1 : "")
    + "\r\n                               value=\""
    + alias2((lookupProperty(helpers,"mapValidStateToBoolean")||(depth0 && lookupProperty(depth0,"mapValidStateToBoolean"))||alias4).call(alias3,(depth0 != null ? lookupProperty(depth0,"isValidForPeriod") : depth0),{"name":"mapValidStateToBoolean","hash":{},"data":data,"loc":{"start":{"line":36,"column":38},"end":{"line":36,"column":86}}}))
    + "\" />\r\n                        <label class=\"govuk-label govuk-checkboxes__label\">\r\n                        </label>\r\n";
},"7":function(container,depth0,helpers,partials,data) {
    return " disabled";
},"9":function(container,depth0,helpers,partials,data) {
    return " checked";
},"11":function(container,depth0,helpers,partials,data) {
    return " greyed-out";
},"13":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, alias3=container.escapeExpression, alias4=container.lambda, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "            "
    + alias3((lookupProperty(helpers,"setVar")||(depth0 && lookupProperty(depth0,"setVar"))||alias2).call(alias1,"masterIndex",(lookupProperty(helpers,"increment")||(depth0 && lookupProperty(depth0,"increment"))||alias2).call(alias1,((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"masterIndex")),{"name":"increment","hash":{},"data":data,"loc":{"start":{"line":47,"column":35},"end":{"line":47,"column":64}}}),{"name":"setVar","hash":{},"data":data,"loc":{"start":{"line":47,"column":12},"end":{"line":47,"column":66}}}))
    + "\r\n            "
    + alias3((lookupProperty(helpers,"setVar")||(depth0 && lookupProperty(depth0,"setVar"))||alias2).call(alias1,"initiatingItem",(lookupProperty(helpers,"isInitiatingItem")||(depth0 && lookupProperty(depth0,"isInitiatingItem"))||alias2).call(alias1,(depth0 != null ? lookupProperty(depth0,"isPausing") : depth0),(depth0 != null ? lookupProperty(depth0,"hasJobs") : depth0),(depth0 != null ? lookupProperty(depth0,"hidden") : depth0),(depth0 != null ? lookupProperty(depth0,"entityType") : depth0),{"name":"isInitiatingItem","hash":{},"data":data,"loc":{"start":{"line":48,"column":38},"end":{"line":48,"column":112}}}),{"name":"setVar","hash":{},"data":data,"loc":{"start":{"line":48,"column":12},"end":{"line":48,"column":115}}}))
    + "\r\n            <li>\r\n                <input type=\"hidden\" class=\"hidden\"\r\n                       id=\"pathitem-"
    + alias3(alias4((depth0 != null ? lookupProperty(depth0,"pathId") : depth0), depth0))
    + "-"
    + alias3(alias4((depth0 != null ? lookupProperty(depth0,"pathItemId") : depth0), depth0))
    + "\"\r\n                       name=\"items["
    + alias3(alias4(((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"masterIndex")), depth0))
    + "].Id\"\r\n                       value=\""
    + alias3(alias4((depth0 != null ? lookupProperty(depth0,"pathItemId") : depth0), depth0))
    + "\" />\r\n\r\n                <input type=\"hidden\" class=\"hidden\"\r\n                       id=\"type-"
    + alias3(alias4((depth0 != null ? lookupProperty(depth0,"pathId") : depth0), depth0))
    + "-"
    + alias3(alias4((depth0 != null ? lookupProperty(depth0,"pathItemId") : depth0), depth0))
    + "\"\r\n                       name=\"items["
    + alias3(alias4(((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"masterIndex")), depth0))
    + "].EntityType\"\r\n                       value=\""
    + alias3(alias4((depth0 != null ? lookupProperty(depth0,"entityType") : depth0), depth0))
    + "\" />\r\n\r\n                <div class=\"checkbox-container\">\r\n                    <div class=\"checkbox-text\">\r\n"
    + ((stack1 = lookupProperty(helpers,"if").call(alias1,(depth0 != null ? lookupProperty(depth0,"hidden") : depth0),{"name":"if","hash":{},"fn":container.program(14, data, 0, blockParams, depths),"inverse":container.program(16, data, 0, blockParams, depths),"data":data,"loc":{"start":{"line":62,"column":24},"end":{"line":66,"column":31}}})) != null ? stack1 : "")
    + "                    </div>\r\n\r\n                    <div class=\"govuk-checkboxes\">\r\n                        <div class=\"flex\">\r\n                            <div class=\"govuk-checkboxes__item\">\r\n"
    + ((stack1 = lookupProperty(helpers,"unless").call(alias1,(depth0 != null ? lookupProperty(depth0,"hidden") : depth0),{"name":"unless","hash":{},"fn":container.program(18, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":72,"column":32},"end":{"line":86,"column":43}}})) != null ? stack1 : "")
    + "                            </div>\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n            </li>\r\n";
},"14":function(container,depth0,helpers,partials,data) {
    var alias1=container.lambda, alias2=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                        "
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"name") : depth0), depth0))
    + " <a href=\"#"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"subPaths") : depth0), depth0))
    + "\" id=\"item_"
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"subPaths") : depth0), depth0))
    + "\">❯</a>\r\n";
},"16":function(container,depth0,helpers,partials,data) {
    var lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                        "
    + container.escapeExpression(container.lambda((depth0 != null ? lookupProperty(depth0,"name") : depth0), depth0))
    + "\r\n";
},"18":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return ((stack1 = lookupProperty(helpers,"unless").call(depth0 != null ? depth0 : (container.nullContext || {}),((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"initiatingItem")),{"name":"unless","hash":{},"fn":container.program(19, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":73,"column":32},"end":{"line":85,"column":43}}})) != null ? stack1 : "");
},"19":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, alias3=container.lambda, alias4=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return ((stack1 = lookupProperty(helpers,"unless").call(alias1,(lookupProperty(helpers,"disableCheckBoxIfNotInPeriod")||(depth0 && lookupProperty(depth0,"disableCheckBoxIfNotInPeriod"))||alias2).call(alias1,(depth0 != null ? lookupProperty(depth0,"isValidForPeriod") : depth0),{"name":"disableCheckBoxIfNotInPeriod","hash":{},"data":data,"loc":{"start":{"line":74,"column":42},"end":{"line":74,"column":94}}}),{"name":"unless","hash":{},"fn":container.program(20, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":74,"column":32},"end":{"line":76,"column":43}}})) != null ? stack1 : "")
    + "\r\n                                <input type=\"checkbox\" class=\"govuk-checkboxes__input validityCheckbox\"\r\n                                       id=\"ckpathItem-"
    + alias4(alias3((depth0 != null ? lookupProperty(depth0,"pathId") : depth0), depth0))
    + "-"
    + alias4(alias3((depth0 != null ? lookupProperty(depth0,"pathItemId") : depth0), depth0))
    + "\"\r\n                                       "
    + ((stack1 = lookupProperty(helpers,"if").call(alias1,(lookupProperty(helpers,"disableCheckboxes")||(depth0 && lookupProperty(depth0,"disableCheckboxes"))||alias2).call(alias1,(depths[1] != null ? lookupProperty(depths[1],"isValidForPeriod") : depths[1]),(depths[1] != null ? lookupProperty(depths[1],"isCritical") : depths[1]),(lookupProperty(helpers,"disableCheckBoxIfNotInPeriod")||(depth0 && lookupProperty(depth0,"disableCheckBoxIfNotInPeriod"))||alias2).call(alias1,(depth0 != null ? lookupProperty(depth0,"isValidForPeriod") : depth0),{"name":"disableCheckBoxIfNotInPeriod","hash":{},"data":data,"loc":{"start":{"line":80,"column":98},"end":{"line":80,"column":150}}}),((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"hasAlreadyRun")),{"name":"disableCheckboxes","hash":{},"data":data,"loc":{"start":{"line":80,"column":45},"end":{"line":80,"column":171}}}),{"name":"if","hash":{},"fn":container.program(7, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":80,"column":39},"end":{"line":80,"column":189}}})) != null ? stack1 : "")
    + "\r\n                                       "
    + ((stack1 = lookupProperty(helpers,"if").call(alias1,(lookupProperty(helpers,"mapValidStateToBoolean")||(depth0 && lookupProperty(depth0,"mapValidStateToBoolean"))||alias2).call(alias1,(depth0 != null ? lookupProperty(depth0,"isValidForPeriod") : depth0),{"name":"mapValidStateToBoolean","hash":{},"data":data,"loc":{"start":{"line":81,"column":45},"end":{"line":81,"column":91}}}),{"name":"if","hash":{},"fn":container.program(9, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":81,"column":39},"end":{"line":81,"column":108}}})) != null ? stack1 : "")
    + "\r\n                                       value=\""
    + alias4((lookupProperty(helpers,"mapValidStateToBoolean")||(depth0 && lookupProperty(depth0,"mapValidStateToBoolean"))||alias2).call(alias1,(depth0 != null ? lookupProperty(depth0,"isValidForPeriod") : depth0),{"name":"mapValidStateToBoolean","hash":{},"data":data,"loc":{"start":{"line":82,"column":46},"end":{"line":82,"column":94}}}))
    + "\" />\r\n                                <label class=\"govuk-label govuk-checkboxes__label\">\r\n                                </label>\r\n";
},"20":function(container,depth0,helpers,partials,data) {
    var stack1, alias1=container.lambda, alias2=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                                <input type=\"hidden\" name=\"items["
    + alias2(alias1(((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"masterIndex")), depth0))
    + "].Enabled\" value=\""
    + alias2(alias1((depth0 != null ? lookupProperty(depth0,"isValidForPeriod") : depth0), depth0))
    + "\" />\r\n";
},"22":function(container,depth0,helpers,partials,data) {
    return "<button type=\"submit\" class=\"govuk-button\" id=\"saveChanges\">Save</button>\r\n";
},"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, alias3=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "﻿<hr />\r\n\r\n<ol class=\"app-task-list__items\">\r\n    "
    + alias3((lookupProperty(helpers,"setVar")||(depth0 && lookupProperty(depth0,"setVar"))||alias2).call(alias1,"masterIndex",0,{"name":"setVar","hash":{},"data":data,"loc":{"start":{"line":4,"column":4},"end":{"line":4,"column":30}}}))
    + "\r\n    "
    + alias3((lookupProperty(helpers,"setVar")||(depth0 && lookupProperty(depth0,"setVar"))||alias2).call(alias1,"hasAlreadyRun",((stack1 = (depth0 != null ? lookupProperty(depth0,"viewModel") : depth0)) != null ? lookupProperty(stack1,"periodEndHasRunForPeriod") : stack1),{"name":"setVar","hash":{},"data":data,"loc":{"start":{"line":5,"column":4},"end":{"line":5,"column":65}}}))
    + "\r\n"
    + ((stack1 = lookupProperty(helpers,"each").call(alias1,((stack1 = (depth0 != null ? lookupProperty(depth0,"viewModel") : depth0)) != null ? lookupProperty(stack1,"paths") : stack1),{"name":"each","hash":{},"fn":container.program(1, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":6,"column":4},"end":{"line":98,"column":13}}})) != null ? stack1 : "")
    + "</ol>\r\n\r\n"
    + ((stack1 = lookupProperty(helpers,"unless").call(alias1,((stack1 = (depth0 != null ? lookupProperty(depth0,"viewModel") : depth0)) != null ? lookupProperty(stack1,"periodEndHasRunForPeriod") : stack1),{"name":"unless","hash":{},"fn":container.program(22, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":101,"column":0},"end":{"line":103,"column":11}}})) != null ? stack1 : "");
},"useData":true,"useDepths":true});
templates['ReferenceData/FilesListTemplate'] = template({"1":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, alias3="function", alias4=container.escapeExpression, alias5=container.lambda, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                <tr class=\"govuk-table__row\">\r\n                    <td class=\"govuk-table__cell govuk-!-font-weight-bold td--width-100-wrap\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"displayDate") || (depth0 != null ? lookupProperty(depth0,"displayDate") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"displayDate","hash":{},"data":data,"loc":{"start":{"line":17,"column":94},"end":{"line":17,"column":109}}}) : helper)))
    + "</td>\r\n                    <td class=\"govuk-table__cell\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"submittedBy") || (depth0 != null ? lookupProperty(depth0,"submittedBy") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"submittedBy","hash":{},"data":data,"loc":{"start":{"line":18,"column":50},"end":{"line":18,"column":65}}}) : helper)))
    + "</td>\r\n                    <td class=\"govuk-table__cell td--width-150-wrap\"><a href=\""
    + alias4(alias5((depths[1] != null ? lookupProperty(depths[1],"downloadController") : depths[1]), depth0))
    + "/"
    + alias4(((helper = (helper = lookupProperty(helpers,"collectionName") || (depth0 != null ? lookupProperty(depth0,"collectionName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"collectionName","hash":{},"data":data,"loc":{"start":{"line":19,"column":104},"end":{"line":19,"column":122}}}) : helper)))
    + "/"
    + alias4(((helper = (helper = lookupProperty(helpers,"fileName") || (depth0 != null ? lookupProperty(depth0,"fileName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"fileName","hash":{},"data":data,"loc":{"start":{"line":19,"column":123},"end":{"line":19,"column":135}}}) : helper)))
    + "\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"fileName") || (depth0 != null ? lookupProperty(depth0,"fileName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"fileName","hash":{},"data":data,"loc":{"start":{"line":19,"column":137},"end":{"line":19,"column":149}}}) : helper)))
    + "</a></td>\r\n                    <td class=\"govuk-table__cell\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"jobId") || (depth0 != null ? lookupProperty(depth0,"jobId") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"jobId","hash":{},"data":data,"loc":{"start":{"line":20,"column":50},"end":{"line":20,"column":59}}}) : helper)))
    + "</td>\r\n                    <td class=\"govuk-table__cell\">\r\n                        <span class=\""
    + alias4((lookupProperty(helpers,"jobStatusClass")||(depth0 && lookupProperty(depth0,"jobStatusClass"))||alias2).call(alias1,(depth0 != null ? lookupProperty(depth0,"displayStatus") : depth0),{"name":"jobStatusClass","hash":{},"data":data,"loc":{"start":{"line":22,"column":37},"end":{"line":22,"column":69}}}))
    + "\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"displayStatus") || (depth0 != null ? lookupProperty(depth0,"displayStatus") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"displayStatus","hash":{},"data":data,"loc":{"start":{"line":22,"column":71},"end":{"line":22,"column":88}}}) : helper)))
    + "</span> <br />\r\n                        <span class=\"govuk-!-font-weight-bold\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"recordCount") || (depth0 != null ? lookupProperty(depth0,"recordCount") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"recordCount","hash":{},"data":data,"loc":{"start":{"line":23,"column":63},"end":{"line":23,"column":78}}}) : helper)))
    + " records</span> <br />\r\n                        <span class=\"govuk-!-font-weight-bold\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"warningCount") || (depth0 != null ? lookupProperty(depth0,"warningCount") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"warningCount","hash":{},"data":data,"loc":{"start":{"line":24,"column":63},"end":{"line":24,"column":79}}}) : helper)))
    + " warnings</span> <br />\r\n                        <span class=\"govuk-!-font-weight-bold\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"errorCount") || (depth0 != null ? lookupProperty(depth0,"errorCount") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"errorCount","hash":{},"data":data,"loc":{"start":{"line":25,"column":63},"end":{"line":25,"column":77}}}) : helper)))
    + " errors</span> <br />\r\n                    </td>\r\n                    <td class=\"govuk-table__cell td--width-150-wrap\"><a href=\""
    + alias4(alias5((depths[1] != null ? lookupProperty(depths[1],"downloadController") : depths[1]), depth0))
    + "/"
    + alias4(((helper = (helper = lookupProperty(helpers,"collectionName") || (depth0 != null ? lookupProperty(depth0,"collectionName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"collectionName","hash":{},"data":data,"loc":{"start":{"line":27,"column":104},"end":{"line":27,"column":122}}}) : helper)))
    + "/"
    + alias4(((helper = (helper = lookupProperty(helpers,"reportName") || (depth0 != null ? lookupProperty(depth0,"reportName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"reportName","hash":{},"data":data,"loc":{"start":{"line":27,"column":123},"end":{"line":27,"column":137}}}) : helper)))
    + "/"
    + alias4(((helper = (helper = lookupProperty(helpers,"jobId") || (depth0 != null ? lookupProperty(depth0,"jobId") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"jobId","hash":{},"data":data,"loc":{"start":{"line":27,"column":138},"end":{"line":27,"column":147}}}) : helper)))
    + "\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"reportName") || (depth0 != null ? lookupProperty(depth0,"reportName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"reportName","hash":{},"data":data,"loc":{"start":{"line":27,"column":149},"end":{"line":27,"column":163}}}) : helper)))
    + "</a></td>\r\n                </tr>\r\n";
},"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "﻿<div class=\"govuk-grid-row\">\r\n    <div class=\"govuk-grid-column-full\">\r\n        <table class=\"govuk-table\">\r\n            <thead class=\"govuk-table__head\">\r\n                <tr class=\"govuk-table__row\">\r\n                    <th scope=\"col\" class=\"govuk-table__header td--width-100-wrap\">Date</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header\">Submitted by</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header td--width-150-wrap\">File</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header\">Job ID</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header table-width-min\">Status</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header td--width-150-wrap\">Reports</th>\r\n                </tr>\r\n            </thead>\r\n            <tbody class=\"govuk-table__body\" id=\"fileContainer\">\r\n"
    + ((stack1 = lookupProperty(helpers,"each").call(depth0 != null ? depth0 : (container.nullContext || {}),((stack1 = (depth0 != null ? lookupProperty(depth0,"viewModel") : depth0)) != null ? lookupProperty(stack1,"files") : stack1),{"name":"each","hash":{},"fn":container.program(1, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":15,"column":16},"end":{"line":29,"column":25}}})) != null ? stack1 : "")
    + "            </tbody>\r\n        </table>\r\n    </div>\r\n</div>";
},"useData":true,"useDepths":true});
templates['ReferenceData/FisFilesListTemplate'] = template({"1":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, alias3="function", alias4=container.escapeExpression, alias5=container.lambda, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                <tr class=\"govuk-table__row\">\r\n                    <td class=\"govuk-table__cell govuk-!-font-weight-bold td--width-100-wrap\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"displayDate") || (depth0 != null ? lookupProperty(depth0,"displayDate") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"displayDate","hash":{},"data":data,"loc":{"start":{"line":17,"column":94},"end":{"line":17,"column":109}}}) : helper)))
    + "</td>\r\n                    <td class=\"govuk-table__cell\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"submittedBy") || (depth0 != null ? lookupProperty(depth0,"submittedBy") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"submittedBy","hash":{},"data":data,"loc":{"start":{"line":18,"column":50},"end":{"line":18,"column":65}}}) : helper)))
    + "</td>\r\n                    <td class=\"govuk-table__cell\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"jobId") || (depth0 != null ? lookupProperty(depth0,"jobId") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"jobId","hash":{},"data":data,"loc":{"start":{"line":19,"column":50},"end":{"line":19,"column":59}}}) : helper)))
    + "</td>\r\n                    <td class=\"govuk-table__cell\">\r\n                        <span class=\""
    + alias4((lookupProperty(helpers,"jobStatusClass")||(depth0 && lookupProperty(depth0,"jobStatusClass"))||alias2).call(alias1,(depth0 != null ? lookupProperty(depth0,"displayStatus") : depth0),{"name":"jobStatusClass","hash":{},"data":data,"loc":{"start":{"line":21,"column":37},"end":{"line":21,"column":69}}}))
    + "\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"displayStatus") || (depth0 != null ? lookupProperty(depth0,"displayStatus") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"displayStatus","hash":{},"data":data,"loc":{"start":{"line":21,"column":71},"end":{"line":21,"column":88}}}) : helper)))
    + "</span> <br />\r\n                    </td>\r\n                    <td class=\"govuk-table__cell td--width-150-wrap\"><a href=\""
    + alias4(alias5((depths[1] != null ? lookupProperty(depths[1],"downloadController") : depths[1]), depth0))
    + "/"
    + alias4(((helper = (helper = lookupProperty(helpers,"collectionName") || (depth0 != null ? lookupProperty(depth0,"collectionName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"collectionName","hash":{},"data":data,"loc":{"start":{"line":23,"column":104},"end":{"line":23,"column":122}}}) : helper)))
    + "/"
    + alias4(((helper = (helper = lookupProperty(helpers,"reportName") || (depth0 != null ? lookupProperty(depth0,"reportName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"reportName","hash":{},"data":data,"loc":{"start":{"line":23,"column":123},"end":{"line":23,"column":137}}}) : helper)))
    + "/"
    + alias4(((helper = (helper = lookupProperty(helpers,"jobId") || (depth0 != null ? lookupProperty(depth0,"jobId") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"jobId","hash":{},"data":data,"loc":{"start":{"line":23,"column":138},"end":{"line":23,"column":147}}}) : helper)))
    + "\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"reportName") || (depth0 != null ? lookupProperty(depth0,"reportName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"reportName","hash":{},"data":data,"loc":{"start":{"line":23,"column":149},"end":{"line":23,"column":163}}}) : helper)))
    + "</a></td>\r\n                    <td class=\"govuk-table__cell td--width-150-wrap\"><a href=\""
    + alias4(alias5((depths[1] != null ? lookupProperty(depths[1],"downloadController") : depths[1]), depth0))
    + "/"
    + alias4(((helper = (helper = lookupProperty(helpers,"collectionName") || (depth0 != null ? lookupProperty(depth0,"collectionName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"collectionName","hash":{},"data":data,"loc":{"start":{"line":24,"column":104},"end":{"line":24,"column":122}}}) : helper)))
    + "/"
    + alias4(((helper = (helper = lookupProperty(helpers,"fileName") || (depth0 != null ? lookupProperty(depth0,"fileName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"fileName","hash":{},"data":data,"loc":{"start":{"line":24,"column":123},"end":{"line":24,"column":135}}}) : helper)))
    + "/"
    + alias4(((helper = (helper = lookupProperty(helpers,"jobId") || (depth0 != null ? lookupProperty(depth0,"jobId") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"jobId","hash":{},"data":data,"loc":{"start":{"line":24,"column":136},"end":{"line":24,"column":145}}}) : helper)))
    + "\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"fileName") || (depth0 != null ? lookupProperty(depth0,"fileName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"fileName","hash":{},"data":data,"loc":{"start":{"line":24,"column":147},"end":{"line":24,"column":159}}}) : helper)))
    + "</a></td>\r\n                </tr>\r\n";
},"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "﻿<div class=\"govuk-grid-row\">\r\n    <div class=\"govuk-grid-column-full\">\r\n        <table class=\"govuk-table\">\r\n            <thead class=\"govuk-table__head\">\r\n                <tr class=\"govuk-table__row\">\r\n                    <th scope=\"col\" class=\"govuk-table__header td--width-100-wrap\">Date</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header\">Processed by</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header\">Job ID</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header table-width-min\">Status</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header td--width-150-wrap\">Report</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header td--width-150-wrap\">File</th>\r\n                </tr>\r\n            </thead>\r\n            <tbody class=\"govuk-table__body\" id=\"fileContainer\">\r\n"
    + ((stack1 = lookupProperty(helpers,"each").call(depth0 != null ? depth0 : (container.nullContext || {}),((stack1 = (depth0 != null ? lookupProperty(depth0,"viewModel") : depth0)) != null ? lookupProperty(stack1,"files") : stack1),{"name":"each","hash":{},"fn":container.program(1, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":15,"column":12},"end":{"line":26,"column":21}}})) != null ? stack1 : "")
    + "            </tbody>\r\n        </table>\r\n    </div>\r\n</div>";
},"useData":true,"useDepths":true});
templates['ReferenceData/FundingClaimsDatesList'] = template({"1":function(container,depth0,helpers,partials,data) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return ((stack1 = lookupProperty(helpers,"if").call(depth0 != null ? depth0 : (container.nullContext || {}),(depth0 != null ? lookupProperty(depth0,"inEditMode") : depth0),{"name":"if","hash":{},"fn":container.program(2, data, 0),"inverse":container.program(8, data, 0),"data":data,"loc":{"start":{"line":21,"column":16},"end":{"line":60,"column":23}}})) != null ? stack1 : "");
},"2":function(container,depth0,helpers,partials,data) {
    var stack1, helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, alias3="function", alias4=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                <input type=\"hidden\" name=\"id\" value=\""
    + alias4(((helper = (helper = lookupProperty(helpers,"id") || (depth0 != null ? lookupProperty(depth0,"id") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"id","hash":{},"data":data,"loc":{"start":{"line":22,"column":54},"end":{"line":22,"column":60}}}) : helper)))
    + "\"/>\r\n                <tr class=\"govuk-table__row row-hide\" id=\"add-table-row\">\r\n                    <td scope=\"row\" class=\"govuk-table__cell fundingclaimdates-edit-width\" >\r\n                        <select class=\"govuk-select .fundingclaimdates-edit-width\" id=\"sort\" name=\"collectionId\">\r\n"
    + ((stack1 = (lookupProperty(helpers,"select")||(depth0 && lookupProperty(depth0,"select"))||alias2).call(alias1,(depth0 != null ? lookupProperty(depth0,"collectionId") : depth0),{"name":"select","hash":{},"fn":container.program(3, data, 0),"inverse":container.noop,"data":data,"loc":{"start":{"line":26,"column":28},"end":{"line":30,"column":39}}})) != null ? stack1 : "")
    + "                        </select>\r\n                    </td>\r\n                    <td class=\"govuk-table__cell\"><input class=\"govuk-input fundingclaimdates-edit-width\" type=\"datetime-local\" id=\"submissionOpenDateUtc\" name=\"submissionOpenDateUtc\" required value="
    + alias4(((helper = (helper = lookupProperty(helpers,"submissionOpenDateUtc") || (depth0 != null ? lookupProperty(depth0,"submissionOpenDateUtc") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"submissionOpenDateUtc","hash":{},"data":data,"loc":{"start":{"line":33,"column":199},"end":{"line":33,"column":224}}}) : helper)))
    + " data-id=\""
    + alias4(((helper = (helper = lookupProperty(helpers,"id") || (depth0 != null ? lookupProperty(depth0,"id") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"id","hash":{},"data":data,"loc":{"start":{"line":33,"column":234},"end":{"line":33,"column":240}}}) : helper)))
    + "\" placeholder=\"DD/MM/YYYY HH:MM\"></td>\r\n                    <td class=\"govuk-table__cell\"><input class=\"govuk-input fundingclaimdates-edit-width\" type=\"datetime-local\" id=\"submissionCloseDateUtc\" name=\"submissionCloseDateUtc\" required value="
    + alias4(((helper = (helper = lookupProperty(helpers,"submissionCloseDateUtc") || (depth0 != null ? lookupProperty(depth0,"submissionCloseDateUtc") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"submissionCloseDateUtc","hash":{},"data":data,"loc":{"start":{"line":34,"column":201},"end":{"line":34,"column":227}}}) : helper)))
    + " data-id=\""
    + alias4(((helper = (helper = lookupProperty(helpers,"id") || (depth0 != null ? lookupProperty(depth0,"id") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"id","hash":{},"data":data,"loc":{"start":{"line":34,"column":237},"end":{"line":34,"column":243}}}) : helper)))
    + "\" placeholder=\"DD/MM/YYYY HH:MM\"></td>\r\n                    <td class=\"govuk-table__cell\"><input class=\"govuk-input fundingclaimdates-edit-width\" type=\"datetime-local\" id=\"helpdeskOpenDateUtc\" name=\"helpdeskOpenDateUtc\" required value="
    + alias4(((helper = (helper = lookupProperty(helpers,"helpdeskOpenDateUtc") || (depth0 != null ? lookupProperty(depth0,"helpdeskOpenDateUtc") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"helpdeskOpenDateUtc","hash":{},"data":data,"loc":{"start":{"line":35,"column":195},"end":{"line":35,"column":218}}}) : helper)))
    + " data-id=\""
    + alias4(((helper = (helper = lookupProperty(helpers,"id") || (depth0 != null ? lookupProperty(depth0,"id") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"id","hash":{},"data":data,"loc":{"start":{"line":35,"column":228},"end":{"line":35,"column":234}}}) : helper)))
    + "\" placeholder=\"DD/MM/YYYY HH:MM\"></td>\r\n                    <td class=\"govuk-table__cell fundingclaimdates-edit-width\">\r\n                        <select class=\"govuk-select .fundingclaimdates-edit-width\" id=\"requiresSignature\" name=\"requiresSignature\">\r\n"
    + ((stack1 = (lookupProperty(helpers,"select")||(depth0 && lookupProperty(depth0,"select"))||alias2).call(alias1,(depth0 != null ? lookupProperty(depth0,"requiresSignature") : depth0),{"name":"select","hash":{},"fn":container.program(6, data, 0),"inverse":container.noop,"data":data,"loc":{"start":{"line":38,"column":28},"end":{"line":41,"column":39}}})) != null ? stack1 : "")
    + "                        </select>\r\n                    </td>\r\n                    <td class=\"govuk-table__cell\"><input class=\"govuk-input fundingclaimdates-edit-width\" type=\"datetime-local\" id=\"signatureCloseDateUtc\" name=\"signatureCloseDateUtc\" value=\""
    + alias4(((helper = (helper = lookupProperty(helpers,"signatureCloseDateUtc") || (depth0 != null ? lookupProperty(depth0,"signatureCloseDateUtc") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"signatureCloseDateUtc","hash":{},"data":data,"loc":{"start":{"line":44,"column":191},"end":{"line":44,"column":216}}}) : helper)))
    + "\" placeholder=\"DD/MM/YYYY HH:MM\"></td>\r\n                    <td class=\"govuk-table__cell govuk-table__header--numeric\"><button class=\"govuk-button\" data-module=\"govuk-button\" data-collectionId=\""
    + alias4(((helper = (helper = lookupProperty(helpers,"collectionId") || (depth0 != null ? lookupProperty(depth0,"collectionId") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"collectionId","hash":{},"data":data,"loc":{"start":{"line":45,"column":154},"end":{"line":45,"column":170}}}) : helper)))
    + "\" data-id=\""
    + alias4(((helper = (helper = lookupProperty(helpers,"id") || (depth0 != null ? lookupProperty(depth0,"id") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"id","hash":{},"data":data,"loc":{"start":{"line":45,"column":181},"end":{"line":45,"column":187}}}) : helper)))
    + "\" id=\"save\">Save</button> <a data-collectionId=\""
    + alias4(((helper = (helper = lookupProperty(helpers,"collectionId") || (depth0 != null ? lookupProperty(depth0,"collectionId") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"collectionId","hash":{},"data":data,"loc":{"start":{"line":45,"column":235},"end":{"line":45,"column":251}}}) : helper)))
    + "\" data-id=\""
    + alias4(((helper = (helper = lookupProperty(helpers,"id") || (depth0 != null ? lookupProperty(depth0,"id") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"id","hash":{},"data":data,"loc":{"start":{"line":45,"column":262},"end":{"line":45,"column":268}}}) : helper)))
    + "\" href=\"#\" id=\"cancel\">Cancel</a> </td>\r\n                </tr>\r\n\r\n";
},"3":function(container,depth0,helpers,partials,data) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return ((stack1 = lookupProperty(helpers,"each").call(depth0 != null ? depth0 : (container.nullContext || {}),((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"collectionPeriods")),{"name":"each","hash":{},"fn":container.program(4, data, 0),"inverse":container.noop,"data":data,"loc":{"start":{"line":27,"column":28},"end":{"line":29,"column":37}}})) != null ? stack1 : "");
},"4":function(container,depth0,helpers,partials,data) {
    var helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, alias3="function", alias4=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                            <option value=\""
    + alias4(((helper = (helper = lookupProperty(helpers,"value") || (depth0 != null ? lookupProperty(depth0,"value") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"value","hash":{},"data":data,"loc":{"start":{"line":28,"column":43},"end":{"line":28,"column":52}}}) : helper)))
    + "\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"text") || (depth0 != null ? lookupProperty(depth0,"text") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"text","hash":{},"data":data,"loc":{"start":{"line":28,"column":54},"end":{"line":28,"column":62}}}) : helper)))
    + "</option>\r\n";
},"6":function(container,depth0,helpers,partials,data) {
    return "                            <option value=\"Y\">Y</option>\r\n                            <option value=\"N\">N</option>\r\n";
},"8":function(container,depth0,helpers,partials,data) {
    var helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, alias3="function", alias4=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                <tr class=\"govuk-table__row\">\r\n                    <td scope=\"row\" class=\"govuk-table__cell\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"collectionName") || (depth0 != null ? lookupProperty(depth0,"collectionName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"collectionName","hash":{},"data":data,"loc":{"start":{"line":50,"column":62},"end":{"line":50,"column":80}}}) : helper)))
    + "</td>\r\n                    <td class=\"govuk-table__cell\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"submissionOpenDateUtcFormattedString") || (depth0 != null ? lookupProperty(depth0,"submissionOpenDateUtcFormattedString") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"submissionOpenDateUtcFormattedString","hash":{},"data":data,"loc":{"start":{"line":51,"column":50},"end":{"line":51,"column":90}}}) : helper)))
    + "</td>\r\n                    <td class=\"govuk-table__cell\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"submissionCloseDateUtcFormattedString") || (depth0 != null ? lookupProperty(depth0,"submissionCloseDateUtcFormattedString") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"submissionCloseDateUtcFormattedString","hash":{},"data":data,"loc":{"start":{"line":52,"column":50},"end":{"line":52,"column":91}}}) : helper)))
    + "</td>\r\n                    <td class=\"govuk-table__cell\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"helpdeskOpenDateUtcFormattedString") || (depth0 != null ? lookupProperty(depth0,"helpdeskOpenDateUtcFormattedString") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"helpdeskOpenDateUtcFormattedString","hash":{},"data":data,"loc":{"start":{"line":53,"column":50},"end":{"line":53,"column":88}}}) : helper)))
    + "</td>\r\n                    <td class=\"govuk-table__cell\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"requiresSignature") || (depth0 != null ? lookupProperty(depth0,"requiresSignature") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"requiresSignature","hash":{},"data":data,"loc":{"start":{"line":54,"column":50},"end":{"line":54,"column":71}}}) : helper)))
    + "</td>\r\n                    <td class=\"govuk-table__cell\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"signatureCloseDateUtcFormattedString") || (depth0 != null ? lookupProperty(depth0,"signatureCloseDateUtcFormattedString") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"signatureCloseDateUtcFormattedString","hash":{},"data":data,"loc":{"start":{"line":55,"column":50},"end":{"line":55,"column":90}}}) : helper)))
    + "</td>\r\n                    <td>\r\n                        <button class=\"govuk-button modify-table-row\" data-module=\"govuk-button\" data-collectionId=\""
    + alias4(((helper = (helper = lookupProperty(helpers,"collectionId") || (depth0 != null ? lookupProperty(depth0,"collectionId") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"collectionId","hash":{},"data":data,"loc":{"start":{"line":57,"column":116},"end":{"line":57,"column":132}}}) : helper)))
    + "\" data-id=\""
    + alias4(((helper = (helper = lookupProperty(helpers,"id") || (depth0 != null ? lookupProperty(depth0,"id") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"id","hash":{},"data":data,"loc":{"start":{"line":57,"column":143},"end":{"line":57,"column":149}}}) : helper)))
    + "\" id=\"modify\">Modify</button>\r\n                    </td>\r\n                </tr>\r\n";
},"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "﻿<div class=\"govuk-grid-row\">\r\n    <div class=\"govuk-grid-column-full\">\r\n        <span id=\"fundingclaimsDates-error\" class=\"govuk-error-message\">\r\n            <span class=\"govuk-visually-hidden\">Error:</span> Validation failed. Please check the dates entered.\r\n        </span>\r\n        <form id=\"fundingClaimsDatesForm\">\r\n            <table class=\"govuk-table\">\r\n                <thead class=\"govuk-table__head\">\r\n                <tr class=\"govuk-table__row\">\r\n                    <th scope=\"col\" class=\"govuk-table__header govuk-width-15\">Data Collection period</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header\">Submission Open Date</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header\">Submission close date</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header govuk-width-17\">Helpdesk open date</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header\">Signature required?</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header\">Signature close date</th>\r\n                    <th scope=\"col\" class=\"govuk-table__header\"></th>\r\n                </tr>\r\n                </thead>\r\n                <tbody class=\"govuk-table__body\">\r\n"
    + ((stack1 = lookupProperty(helpers,"each").call(depth0 != null ? depth0 : (container.nullContext || {}),(depth0 != null ? lookupProperty(depth0,"viewModel") : depth0),{"name":"each","hash":{},"fn":container.program(1, data, 0),"inverse":container.noop,"data":data,"loc":{"start":{"line":20,"column":16},"end":{"line":61,"column":25}}})) != null ? stack1 : "")
    + "                </tbody>\r\n            </table>\r\n        </form>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['Reports/InternalReportsDownloadList'] = template({"1":function(container,depth0,helpers,partials,data) {
    var stack1, helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, alias3="function", alias4=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "                <tr class=\"govuk-table__row internalreports\">\r\n                    <td class=\"govuk-table__cell\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"displayName") || (depth0 != null ? lookupProperty(depth0,"displayName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"displayName","hash":{},"data":data,"loc":{"start":{"line":14,"column":50},"end":{"line":14,"column":65}}}) : helper)))
    + "</td>\r\n                    <td class=\"govuk-table__cell\"><a href= "
    + alias4((lookupProperty(helpers,"encodedReportUrl")||(depth0 && lookupProperty(depth0,"encodedReportUrl"))||alias2).call(alias1,depth0,{"name":"encodedReportUrl","hash":{"reportDisplayName":(depth0 != null ? lookupProperty(depth0,"displayName") : depth0),"url":(depth0 != null ? lookupProperty(depth0,"url") : depth0),"downloadUrl":((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"downloadUrl")),"periodSelected":((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"periodSelected")),"yearSelected":((stack1 = (data && lookupProperty(data,"root"))) && lookupProperty(stack1,"yearSelected"))},"data":data,"loc":{"start":{"line":15,"column":59},"end":{"line":15,"column":230}}}))
    + "> "
    + alias4(((helper = (helper = lookupProperty(helpers,"url") || (depth0 != null ? lookupProperty(depth0,"url") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"url","hash":{},"data":data,"loc":{"start":{"line":15,"column":232},"end":{"line":15,"column":239}}}) : helper)))
    + "</a></td>\r\n                </tr>\r\n";
},"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "﻿<div class=\"govuk-grid-row\">\r\n    <div class=\"govuk-grid-column-two-thirds\">\r\n        <h2 class=\"govuk-heading-m\">Internal Reports</h2>\r\n        <table class=\"govuk-table\" id=\"internalReports\">\r\n            <thead class=\"govuk-table__head\">\r\n                <tr class=\"govuk-table__row\">\r\n                    <th class=\"govuk-table__header\" scope=\"col\">Report name</th>\r\n                    <th class=\"govuk-table__header\" scope=\"col\">Download</th>\r\n                </tr>\r\n            </thead>\r\n            <tbody id=\"internalReportsBody\" class=\"govuk-table__body\">\r\n"
    + ((stack1 = lookupProperty(helpers,"each").call(depth0 != null ? depth0 : (container.nullContext || {}),((stack1 = (depth0 != null ? lookupProperty(depth0,"viewModel") : depth0)) != null ? lookupProperty(stack1,"reportUrlDetails") : stack1),{"name":"each","hash":{},"fn":container.program(1, data, 0),"inverse":container.noop,"data":data,"loc":{"start":{"line":12,"column":16},"end":{"line":17,"column":25}}})) != null ? stack1 : "")
    + "            </tbody>\r\n        </table>\r\n    </div>\r\n</div>";
},"useData":true});
templates['Reports/ReportListOptions'] = template({"1":function(container,depth0,helpers,partials,data) {
    var helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, alias3="function", alias4=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "<option value=\""
    + alias4(((helper = (helper = lookupProperty(helpers,"reportName") || (depth0 != null ? lookupProperty(depth0,"reportName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"reportName","hash":{},"data":data,"loc":{"start":{"line":2,"column":15},"end":{"line":2,"column":29}}}) : helper)))
    + "\">"
    + alias4(((helper = (helper = lookupProperty(helpers,"displayName") || (depth0 != null ? lookupProperty(depth0,"displayName") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"displayName","hash":{},"data":data,"loc":{"start":{"line":2,"column":31},"end":{"line":2,"column":46}}}) : helper)))
    + "</option>\r\n";
},"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "﻿"
    + ((stack1 = lookupProperty(helpers,"each").call(depth0 != null ? depth0 : (container.nullContext || {}),((stack1 = (depth0 != null ? lookupProperty(depth0,"viewModel") : depth0)) != null ? lookupProperty(stack1,"availableReportsList") : stack1),{"name":"each","hash":{},"fn":container.program(1, data, 0),"inverse":container.noop,"data":data,"loc":{"start":{"line":1,"column":1},"end":{"line":3,"column":9}}})) != null ? stack1 : "");
},"useData":true});
templates['PeriodEnd/Partials/ILRPathHeader'] = template({"1":function(container,depth0,helpers,partials,data) {
    var helper, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "        <a href=\"#"
    + container.escapeExpression(((helper = (helper = lookupProperty(helpers,"parentPathName") || (depth0 != null ? lookupProperty(depth0,"parentPathName") : depth0)) != null ? helper : container.hooks.helperMissing),(typeof helper === "function" ? helper.call(depth0 != null ? depth0 : (container.nullContext || {}),{"name":"parentPathName","hash":{},"data":data,"loc":{"start":{"line":4,"column":18},"end":{"line":4,"column":36}}}) : helper)))
    + "\">❮ </a>\r\n";
},"3":function(container,depth0,helpers,partials,data) {
    return "⌛";
},"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, alias1=container.lambda, alias2=container.escapeExpression, alias3=depth0 != null ? depth0 : (container.nullContext || {}), lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "﻿<a id=\"Path"
    + alias2(alias1(((stack1 = (depth0 != null ? lookupProperty(depth0,"path") : depth0)) != null ? lookupProperty(stack1,"pathId") : stack1), depth0))
    + "\"></a>\r\n<h3>\r\n"
    + ((stack1 = lookupProperty(helpers,"if").call(alias3,((stack1 = (depth0 != null ? lookupProperty(depth0,"path") : depth0)) != null ? lookupProperty(stack1,"pathId") : stack1),{"name":"if","hash":{},"fn":container.program(1, data, 0),"inverse":container.noop,"data":data,"loc":{"start":{"line":3,"column":4},"end":{"line":5,"column":11}}})) != null ? stack1 : "")
    + "    "
    + alias2(alias1(((stack1 = (depth0 != null ? lookupProperty(depth0,"path") : depth0)) != null ? lookupProperty(stack1,"name") : stack1), depth0))
    + " "
    + ((stack1 = lookupProperty(helpers,"if").call(alias3,((stack1 = (depth0 != null ? lookupProperty(depth0,"path") : depth0)) != null ? lookupProperty(stack1,"isBusy") : stack1),{"name":"if","hash":{},"fn":container.program(3, data, 0),"inverse":container.noop,"data":data,"loc":{"start":{"line":6,"column":18},"end":{"line":6,"column":45}}})) != null ? stack1 : "")
    + "\r\n</h3>";
},"useData":true});
templates['PeriodEnd/Partials/PathHeader'] = template({"1":function(container,depth0,helpers,partials,data) {
    return "⌛";
},"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, alias1=container.lambda, alias2=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "﻿<a id=\"Path"
    + alias2(alias1(((stack1 = (depth0 != null ? lookupProperty(depth0,"path") : depth0)) != null ? lookupProperty(stack1,"pathId") : stack1), depth0))
    + "\"></a>\r\n<h3>\r\n    "
    + alias2(alias1(((stack1 = (depth0 != null ? lookupProperty(depth0,"path") : depth0)) != null ? lookupProperty(stack1,"name") : stack1), depth0))
    + " "
    + ((stack1 = lookupProperty(helpers,"if").call(depth0 != null ? depth0 : (container.nullContext || {}),((stack1 = (depth0 != null ? lookupProperty(depth0,"path") : depth0)) != null ? lookupProperty(stack1,"isBusy") : stack1),{"name":"if","hash":{},"fn":container.program(1, data, 0),"inverse":container.noop,"data":data,"loc":{"start":{"line":3,"column":18},"end":{"line":3,"column":45}}})) != null ? stack1 : "")
    + "\r\n</h3>";
},"useData":true});
templates['PeriodEnd/Partials/PathItemJobSummary'] = template({"1":function(container,depth0,helpers,partials,data) {
    var stack1, alias1=container.lambda, alias2=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "    <li class=\"app-task-list__summary\">\r\n        "
    + alias2(alias1(((stack1 = (depth0 != null ? lookupProperty(depth0,"summary") : depth0)) != null ? lookupProperty(stack1,"numberOfWaitingJobs") : stack1), depth0))
    + " Waiting, "
    + alias2(alias1(((stack1 = (depth0 != null ? lookupProperty(depth0,"summary") : depth0)) != null ? lookupProperty(stack1,"numberOfRunningJobs") : stack1), depth0))
    + " Running, "
    + alias2(alias1(((stack1 = (depth0 != null ? lookupProperty(depth0,"summary") : depth0)) != null ? lookupProperty(stack1,"numberOfFailedJobs") : stack1), depth0))
    + " Failed, "
    + alias2(alias1(((stack1 = (depth0 != null ? lookupProperty(depth0,"summary") : depth0)) != null ? lookupProperty(stack1,"numberOfCompleteJobs") : stack1), depth0))
    + " Complete\r\n    </li>\r\n";
},"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "﻿"
    + ((stack1 = lookupProperty(helpers,"if").call(depth0 != null ? depth0 : (container.nullContext || {}),(depth0 != null ? lookupProperty(depth0,"summary") : depth0),{"name":"if","hash":{},"fn":container.program(1, data, 0),"inverse":container.noop,"data":data,"loc":{"start":{"line":1,"column":1},"end":{"line":5,"column":7}}})) != null ? stack1 : "");
},"useData":true});
templates['PeriodEnd/Partials/ProceedableItemWrapper'] = template({"1":function(container,depth0,helpers,partials,data) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "<div class=\"app-task-list__item\">\r\n"
    + ((stack1 = container.invokePartial(lookupProperty(partials,"@partial-block"),depth0,{"name":"@partial-block","data":data,"indent":"    ","helpers":helpers,"partials":partials,"decorators":container.decorators})) != null ? stack1 : "")
    + ((stack1 = container.invokePartial(lookupProperty(partials,"proceedButton"),depth0,{"name":"proceedButton","hash":{"isBusy":(depth0 != null ? lookupProperty(depth0,"isBusy") : depth0),"isNextItemSubPath":(depth0 != null ? lookupProperty(depth0,"isNextItemSubPath") : depth0),"yearPeriod":(depth0 != null ? lookupProperty(depth0,"yearPeriod") : depth0),"pathItem":depth0},"data":data,"indent":"    ","helpers":helpers,"partials":partials,"decorators":container.decorators})) != null ? stack1 : "")
    + "</div>\r\n";
},"3":function(container,depth0,helpers,partials,data) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return ((stack1 = container.invokePartial(lookupProperty(partials,"@partial-block"),depth0,{"name":"@partial-block","data":data,"indent":"   ","helpers":helpers,"partials":partials,"decorators":container.decorators})) != null ? stack1 : "");
},"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "﻿"
    + ((stack1 = lookupProperty(helpers,"if").call(depth0 != null ? depth0 : (container.nullContext || {}),(depth0 != null ? lookupProperty(depth0,"isProceedable") : depth0),{"name":"if","hash":{},"fn":container.program(1, data, 0),"inverse":container.program(3, data, 0),"data":data,"loc":{"start":{"line":1,"column":1},"end":{"line":8,"column":7}}})) != null ? stack1 : "");
},"usePartial":true,"useData":true});
templates['PeriodEnd/Partials/ProceedButton'] = template({"1":function(container,depth0,helpers,partials,data) {
    return " disabled ";
},"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, alias1=container.lambda, alias2=container.escapeExpression, alias3=depth0 != null ? depth0 : (container.nullContext || {}), alias4=container.hooks.helperMissing, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "﻿<li class=\"app-task-list__item\" id=\"PL_"
    + alias2(alias1(((stack1 = (depth0 != null ? lookupProperty(depth0,"pathItem") : depth0)) != null ? lookupProperty(stack1,"pathItemId") : stack1), depth0))
    + "\">\r\n    <span class=\"inline\">\r\n        <button type=\"submit\" onclick=\"window.periodEndClient.proceed("
    + alias2(alias1(((stack1 = (depth0 != null ? lookupProperty(depth0,"yearPeriod") : depth0)) != null ? lookupProperty(stack1,"year") : stack1), depth0))
    + ","
    + alias2(alias1(((stack1 = (depth0 != null ? lookupProperty(depth0,"yearPeriod") : depth0)) != null ? lookupProperty(stack1,"period") : stack1), depth0))
    + ","
    + alias2(alias1(((stack1 = (depth0 != null ? lookupProperty(depth0,"pathItem") : depth0)) != null ? lookupProperty(stack1,"pathId") : stack1), depth0))
    + ","
    + alias2(alias1(((stack1 = (depth0 != null ? lookupProperty(depth0,"pathItem") : depth0)) != null ? lookupProperty(stack1,"pathItemId") : stack1), depth0))
    + ");this.disabled=true;\" class=\"proceed\" id=\"proceed_"
    + alias2(alias1(((stack1 = (depth0 != null ? lookupProperty(depth0,"pathItem") : depth0)) != null ? lookupProperty(stack1,"pathItemId") : stack1), depth0))
    + "\" "
    + ((stack1 = lookupProperty(helpers,"unless").call(alias3,(lookupProperty(helpers,"canContinue")||(depth0 && lookupProperty(depth0,"canContinue"))||alias4).call(alias3,(depth0 != null ? lookupProperty(depth0,"pathItem") : depth0),(depth0 != null ? lookupProperty(depth0,"isBusy") : depth0),{"name":"canContinue","hash":{},"data":data,"loc":{"start":{"line":3,"column":241},"end":{"line":3,"column":270}}}),{"name":"unless","hash":{},"fn":container.program(1, data, 0),"inverse":container.noop,"data":data,"loc":{"start":{"line":3,"column":231},"end":{"line":3,"column":293}}})) != null ? stack1 : "")
    + ">\r\n            "
    + alias2((lookupProperty(helpers,"getProceedButtonText")||(depth0 && lookupProperty(depth0,"getProceedButtonText"))||alias4).call(alias3,((stack1 = (depth0 != null ? lookupProperty(depth0,"pathItem") : depth0)) != null ? lookupProperty(stack1,"pathItemJobs") : stack1),(depth0 != null ? lookupProperty(depth0,"isNextItemSubPath") : depth0),{"name":"getProceedButtonText","hash":{},"data":data,"loc":{"start":{"line":4,"column":12},"end":{"line":4,"column":76}}}))
    + "\r\n        </button>\r\n        <label style=\"font-size: 10px;\">"
    + alias2((lookupProperty(helpers,"getProceedLabelText")||(depth0 && lookupProperty(depth0,"getProceedLabelText"))||alias4).call(alias3,(depth0 != null ? lookupProperty(depth0,"pathItem") : depth0),(depth0 != null ? lookupProperty(depth0,"isNextItemSubPath") : depth0),{"name":"getProceedLabelText","hash":{},"data":data,"loc":{"start":{"line":6,"column":40},"end":{"line":6,"column":90}}}))
    + "</label>\r\n    </span>\r\n</li>";
},"useData":true});
})();