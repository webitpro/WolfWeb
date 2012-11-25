function hoverOut()
{
    $(this).find('.sectionList').css({ visibility: "hidden" }).hide(200);
    $(this).removeClass('over');
}


function hoverIn()
{    
        //$(this).find('.sectionList').css({ visibility: "visible", display: "none" }).show(200);
        var tabs = ['tab_1', 'tab_2', 'tab_3', 'tab_4', 'tab_5', 'tab_6'];
        var section = $(this).find('.sectionList');
        var childrenCount = parseInt(section.children('li.section').length);
        $(this).addClass('over');
        

        var leftPos = 0;
        var tab = '';
        for (i = 0; i < tabs.length; i++)
        {
            if (section.hasClass(tabs[i]))
            {
                tab = tabs[i];
            }
        }

        switch (tab)
        {
            case 'tab_1':
                break;
            case 'tab_2':
                if (childrenCount > 5)
                {
                    leftPos = 0;
                }
                else
                {
                    leftPos = $(this).position().left + 2; //space for 4 before moving left
                }
                break;
            case 'tab_3':
                if (childrenCount > 4)
                {
                    leftPos = $(this).prev().position().left;
                    if (childrenCount > 3)
                    {
                        leftPos = 0;
                    }
                }
                else
                {
                    leftPos = $(this).position().left + 2; //space for 3 before moving left
                }
                break;
            case 'tab_4':
                var tab4Offset = 145;

                if ($.browser.mozilla || $.browser.msie)
                {
                    tab4Offset = 143;

                    if ($.browser.msie)
                    {
                        tab4Offset = 145;

                        if ($.browser.version == 8)
                        {
                            tab4Offset = 146;
                        }
                    }
                }
                else
                {
                    tab4Offset = 146;
                }
                leftPos = $(this).position().left - $(section).width() + tab4Offset;

                break;
            case 'tab_5':
                var tab5Offset = 130;
                if ($.browser.mozilla || $.browser.msie)
                {
                    tab5Offset = 130;
                    if ($.browser.msie)
                    {
                        if ($.browser.version == 8)
                        {
                            tab5Offset = 131;
                        }
                    }
                }
                else 
                {
                    tab5Offset = 130;
                }

                leftPos = $(this).position().left - $(section).width() + tab5Offset; 
                 
                break;
            case 'tab_6':
                if (childrenCount > 0)
                {
                    leftPos = $(this).position().left;
                    if (childrenCount > 2)
                    {
                        leftPos = $(this).prev().prev().position().left;
                        if (childrenCount > 3)
                        {
                            leftPos = $(this).prev().prev().prev().position().left;
                            if (childrenCount > 4)
                            {
                                leftPos = $(this).prev().prev().prev().prev().position().left;
                                if (childrenCount > 5)
                                {
                                    leftPos = 0;

                                }
                            }
                        }
                    }
                }
                //calculate width and position this ul
                var endPoint = $(this).position().left + 148; //852 
                if (fullWidth == 0)
                {
                    for (i = 0; i < childrenCount; i++)
                    {
                        if ($.browser.msie) 
                        {
                            if ($.browser.version == 8)
                            {
                                //IE8
                                var txt = $('#section_6_' + i.toString()).html().length * 7; 
                                if (i == 0)
                                {
                                    fullWidth = txt;
                                }
                            }
                            else
                            {
                                //IE9
                                fullWidth += $('#section_6_' + i.toString()).outerWidth(true);
                            }
                        }
                        else
                        {
                            fullWidth += $('#section_6_' + i.toString()).outerWidth(true);
                        }

                    }
                }

                var diff = 0;
                if ($.browser.mozilla)
                {
                    diff = 26;
                }
                if ($.browser.msie)
                {
                    if ($.browser.version == 8)
                    {
                        diff = 13;
                    }
                }

                leftPos = endPoint - fullWidth;

                var widthSize = endPoint - leftPos + diff;


                leftPos += 22 - diff;

                section.css({ width: widthSize.toString() + 'px', left: leftPos.toString() + 'px', visibility: "visible", display: "none" }).show(200);

                break;
        }

        if (tab != 'tab_6')
        {
            section.css({ width: "auto", left: leftPos.toString() + 'px', visibility: "visible", display: "none" }).show(200);
        }
}
