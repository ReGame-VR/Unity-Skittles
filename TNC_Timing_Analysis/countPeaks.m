function numPeaks = countPeaks(type, id, trialNum)
%%% Written by Emily Chicklis (echicklis@gmail.com) for Dr. Levac's K01 Skittles study
%%% where type is the type of participant ('TD', 'NU', or 'CP') 
%%% where id is the participant Id (e.g. 'K01_TD_08')
%%% where trialNum is the trial recording number (e.g. '01' or '02')

folder = sprintf('K01_%s', type); 
filepath = sprintf('C:/Program Files (x86)/Vicon/Nexus2.6/Sessions/%s/%s/', folder, id);  
filenames = dir(fullfile(filepath, '**', 'Visit*')); 
sessionFolder = filenames(1).name; 

filepath = fullfile(filepath, sessionFolder); 
addpath(filepath); 

% load file - either move marker data file to the same directory as this script,
% or else load the path containing the .csv file before this line!
opts = detectImportOptions([id ' Trial ' trialNum '.csv'], 'NumHeaderLines', 3); % ignore header lines in file, but include names on row 4
t = readtable([id ' Trial ' trialNum '.csv'], opts);
t([1],:) = []; % delete units row

% extract acceleration data columns
t2 = t(:,{'X__', 'Y__', 'Z__'});
D = table2array(t2); % extract numeric data (no headers needed) 

numPeaks = zeros(1,3); % initialize container for values 

for i = 1:3 % find total number of peaks and valleys in acceleration for each coordinate
    
    [Maxima] = findpeaks(D(:,i)); % get local maxima of data
    DInvert = max(D(:,i)) - D(:,i); % invert data - min values become max values
    [Minima] = findpeaks(DInvert); % get local minima of data (max values of inverted data)
    
    numMax = length(Maxima); % count number of max values
    numMin = length(Minima); % count number of min values
 
    if i == 1
        disp('Peaks for X coordinate');
    elseif i == 2
        disp('Peaks for Y coordinate');
    else
        disp('Peaks for Z coordinate');
    end
    
    numPeaks(i) = numMax + numMin; % sum to return total number of peaks
    disp(numPeaks(i));  
      
end

end