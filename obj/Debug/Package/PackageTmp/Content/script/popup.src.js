var delim = ",";

function openSortWindow(e, id, title) {
    $('#' + id).css('left', $(e).position().left);
    $('#' + id).css('top', $(e).position().top + 20);
    $('#' + id).show();
    $('#' + id + ' div.header').html(title);
}

function closeSortWindow(id) {
    $('#' + id).hide();
}

function up(id) {
    var selectedItem = $('#' + id + ' option:selected');
    var prevItem = selectedItem.prev("option");
    if ($(prevItem).text() != "") {
        $(selectedItem).remove();
        $(prevItem).before($(selectedItem));
    }
    
}

function down(id) {
    var selectedItem = $('#' + id + ' option:selected');
    var nextItem = selectedItem.next('option');
    if ($(nextItem).text() != "") {
        $(selectedItem).remove();
        $(nextItem).after($(selectedItem));
    }
}

function save(id, windowId, option, parentStructureId) {
    switch(option) {
        case 'tab':
            saveTabs(id, windowId);
            break;
        case 'section':
            saveSections(parentStructureId, id, windowId);
            break;
        case 'link':
            saveLinks(parentStructureId, id, windowId);
            break;
    }
}

function saveTabs(id, windowId) {
    var order = getOrder(id, delim);

    $.ajax({
        url: "/Admin/Tab/SaveTabOrder/",
        data: { "order": order, "delim": delim },
        dataType: "json",
        success: function (r) {
            if (r.Success) {
                closeSortWindow(windowId);
                reload();
            }
            else {
                alert(r.Error);
            }
        }
    });
}

function saveSections(tabId, id, windowId) {
    var order = getOrder(id, delim);

    $.ajax({
        url: "/Admin/Section/SaveSectionOrder/",
        data: { "tabId": tabId, "order": order, "delim": delim },
        dataType: "json",
        success: function (r) {
            if (r.Success) {
                closeSortWindow(windowId);
                reload();
            }
            else {
                alert(r.Error);
            }
        }
    });

}

function saveLinks(sectionId, id, windowId) {
    var order = getOrder(id, delim);
    $.ajax({
        url: "/Admin/Link/SaveLinkOrder/",
        data: { "sectionId": sectionId, "order": order, "delim": delim },
        dataType: "json",
        success: function(r)
        {
            if(r.Success){
                closeSortWindow(windowId);
                reload();
            }
            else{
                alert(r.Error);
            }
        }
    });
}


function getOrder(id, delim) {
    var order = "";
    var option = $('#' + id + ' option');
    option.each(function (index) {
        order += delim + $(this).val();
    });
    order = ((order.length >= 1) ? order.substr(1) : order);

    return order;
}
