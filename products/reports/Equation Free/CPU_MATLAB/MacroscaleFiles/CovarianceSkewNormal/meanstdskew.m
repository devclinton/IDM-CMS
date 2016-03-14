function [mn, sd, sk] = meanstdskew(data)

mn = mean(data);
sd = std(data);
sk = mean((data-mn).^3)/sd^3;


end