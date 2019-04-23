inputdir = 'C:/Users/regamevrlab/Documents/MATLAB/Skittle/Codes to use/Bin files';
S = dir(fullfile(inputdir, '*.bin'));
for k = 64
    fnm = fullfile(inputdir, S(k).name);
    fnm
    [filepath,name,ext] = fileparts(fnm); 
    name
    transformTrajectoryData(name); 
end