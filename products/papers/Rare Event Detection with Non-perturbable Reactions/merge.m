function outvec = merge(vec, kappa_min, single_index)
outvec = cell(1,numel(vec));

for oi = 1:numel(vec)
    if ismember(oi, single_index)
        outvec{oi} = {[1 1]};
        continue;
    end    
    
    outi = {};
    ci = 1;
    si = 1;
    cs = 0;
    leftsum = inf;
    voi = vec{oi};
    voilen = length(voi);
    
    while ci <= voilen
        if sum(voi) < kappa_min
            outi = {[1 voilen]};
            break
        end
        
        if voi(ci) >= leftsum
            if voi(ci) >= leftsum + cs || (ci-si)==1
                outi{end} = [outi{end}(1) ci-1];
                outi{end+1} = [ci ci];
                leftsum = voi(ci);
                cs = 0;
                ci = ci + 1;
                si = ci;
            else
                % there may be better partioning of indices
                diff = voi(ci) - leftsum;
                tvec = voi(si:ci-1);
                ind = find(cumsum(tvec) > diff, 1) + si - 1;
                outi{end} = [outi{end}(1) ind];
                outi{end+1} = [ind+1 ci];
                leftsum = sum(voi(ind+1:ci));
                cs = 0;
                ci = ci + 1;
                si = ci;
            end
            continue;
        end
        
        cs = cs + voi(ci);
        
        if cs >= kappa_min
            if cs < leftsum
                outi{end+1} = [si ci];
                leftsum = cs;
                ci = ci+1;
                si = ci;
                cs = 0;
                continue
            else
                % work backward to find cs > lsum
                tsum = 0;
                for i=ci:-1:si
                    tsum = tsum+voi(i);
                    if tsum >= leftsum
                        ind = i;
                        break
                    end
                end
                if ind == si
                    outi{end+1} = [si ci];
                    leftsum = cs;
                else
                    outi{end} = [outi{end}(1) ind-1];
                    outi{end+1} = [ind ci];
                    leftsum = sum(voi(ind:ci));
                end
                ci = ci+1;
                si = ci;
                cs = 0;
                continue
            end
        end
        
        if ci == voilen
            outi{end} = [outi{end}(1) voilen];
            break
        end
        
        ci = ci + 1;
    end
    outvec{oi} = outi;
end