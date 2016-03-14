function obj = generateParamString(paramList,buildList)
    
    obj = generateParamStringHelper({{}},buildList,1);
    obj = generateParamStringHelper(obj,paramList,1);

end


function currStringList = generateParamStringHelper(currStringList, paramList,level)

    if level > length(paramList)
        return
    else
        exist = max(length(currStringList),1);
        options = length(paramList{level}{2}); % number of options at this stage
        
        currStringListNew = {};
        for ii = 1:exist
            for jj = 1:options
                currStringListNew{end+1} = currStringList{ii};
                currStringListNew{end}{end+1} = paramList{level}{2}{jj};
            end
        end
        
        
        currStringListNew = generateParamStringHelper(currStringListNew, paramList, level+1); 
        currStringList = currStringListNew;
    end

end