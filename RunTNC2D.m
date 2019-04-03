function RunTNC2D(dirName, id, hand, session, computed)
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%% code by Emily Chicklis (echicklis@gmail.com) for MATLAB R2018,
%%% based on Action Lab TNC codes

%%% Function Inputs:

%%% fileName = parent file for data: 'NU', 'TD', or 'CP')

%%% id = participant id

%%% hand = 'R' or 'L' - participant's dominant hand

%%% session = 1, 2, or 3 (original 200 trials from Session 1, retention, transfer)
%%% session = 4 is real life, but we haven't gotten there yet

%%% computed = 1 UNLESS the exisitng matrix options in SolMan Figures won't
%%% work for the data to be analyzed. If this is the case, make appropriate
%%% edits in generateSolM and toleranceCost functions, and then change
%%% computed to 0 

%%% For Dr. Levac's K01 project, computed = 1 will generally be correct and doesn't
%%% need editing
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%%% TO EXCLUDE SENSOR ERRORS - uncomment code 2 lines below!
%%% EX: if you need to get rid of trials 1 and 13, use: releaseData([1,13],:) = [];
% releaseData(5,:) = [];
%%%

%%% original and retention task use same target location; transfer uses new
%%% location; adjust for participant's handedness
if session == 1 || session == 2
    ytarget = .43;
    
    if hand == 'R'
        xtarget = .38;
    else
        xtarget = - .38;
    end
    
else % transfer task
    ytarget = .5;
    
    if hand == 'R'
        xtarget = .2;
    else
        xtarget = - .2;
    end
end

TNC_Data = table(); %empty table to hold cost data

%     newFile = newParticipant(dirName, id, session);

%%% concatenate participant data 
    if session == 1
        newFile = sprintf('%s_session1.mat', id);
    elseif session == 2
        newFile = sprintf('%s_session2_retention.mat', id);
    elseif session == 3
        newFile = sprintf('%s_session2_transfer.mat', id);
        %     elseif session == 4
        %         newFile = sprintf('%s_reallifefile.mat', id);
    end
%%%
    
load(fullfile(dirName, newFile)); %%%load concatenated session data

for counter = 1:3 %1 = abs value of velocity, 2 = positive velocities, 3 = negative velocities
    
    AVD = []; %reset AVD between iterations of for loop
    
    % Generate the solution manifold first. If you have a previously made
    % image, load this in generateSolM
    % It will take several minutes to generate the solution manifold if you
    % don't load a previously made file
    
    %%% computed = 1 to load previously made manifold
    %%% computed = 0 to generate from scratch 
    D =  generateSolM([xtarget, ytarget], session, hand, counter, computed);
    
    %%% set angle values
    AVD(:,1) = releaseData(:,2); %angle
    
    %%% set velocity values
    if counter == 1 % absolute value velocities (all throws)
        AVD(:,2) = abs(releaseData(:,3)); %velocity
        disp('Costs for All Velocities (Absolute Value)');
        
    elseif counter == 2 % positive velocities only
        AVD(:,2) = releaseData(:,3); %velocity
        
        pos_indices = find(AVD(:,2)>=0);
        if isempty(pos_indices) == 1
            continue
        end
        
        AVD = AVD(pos_indices,:);
        disp('Costs for Positive Velocities');
        
    else % negative velocities only
        AVD(:,2) = releaseData(:,3); %velocity
        
        neg_indices = find(AVD(:,2)<0);
        if isempty(neg_indices) == 1
            continue
        end
        
        AVD = AVD(neg_indices,:);
        
        disp('Costs for Negative Velocities');
        
    end
    
    %%% set error values
    for ind = 1:size(AVD,1)
        AVD(ind,3) = execution2result_polar_regular(AVD(ind,1),AVD(ind,2),xtarget,ytarget);
    end
    %     AVD(:,3) = releaseData(:,4); % error calculated during task -
    %     better to use the error calculated in the polar function though,
    %     otherwise error will be inconsistent with that used to generate
    %     the noise cost shrink cloud
    
    %%% generate tolerance cost and plot actual and ideal points over solMan
    [Actual, Ideal, TCost] = toleranceCost(0,AVD,D, counter);
    disp(TCost);
    figure(counter),
    hold on
    plot(Actual(:,1),Actual(:,2),'om','markerfacecolor','k');
    plot(Ideal(:,1),Ideal(:,2),'or');
    
    %%% generate noise cost and plot ideal points over solMan
    [Actual, Ideal, NCost] = noiseCost(0,AVD,xtarget,ytarget);
    disp(NCost);
    figure(counter),
    hold on
    plot(Ideal(:,1),Ideal(:,2),'og');
    
    %%% generate covariance cost and plot ideal points over solMan
    [Actual, Ideal, CCost] = covariationCost(0,AVD,xtarget,ytarget);
    disp(CCost);
    figure(counter),
    hold on
    plot(Ideal(:,1),Ideal(:,2),'ob');
    
    %%%prepare cost data to save for each group of throw types
    if counter == 1
        TNC_Data = addvars(TNC_Data, id, session, TCost, NCost, CCost);
        figure_file_suffix = 'abs';
        
    elseif counter == 2
        TCost_pos = TCost; NCost_pos = NCost; CCost_pos = CCost;
        TNC_Data = addvars(TNC_Data, TCost_pos, NCost_pos, CCost_pos);
        figure_file_suffix = 'pos';
        
    else
        TCost_neg = TCost; NCost_neg = NCost; CCost_neg = CCost;
        TNC_Data = addvars(TNC_Data, TCost_neg, NCost_neg, CCost_neg);
        figure_file_suffix = 'neg';
        
    end
    
    %%% save figures with plotted points
    if session == 1
        saveas(figure(counter),[pwd sprintf('/SolMan Figures/%s figs/%s_session1_%s.fig', dirName, id, figure_file_suffix)]);
    elseif session == 2
        saveas(figure(counter),[pwd sprintf('/SolMan Figures/%s figs/%s_session2_retention_%s.fig', dirName, id, figure_file_suffix)]);
    elseif session == 3
        saveas(figure(counter),[pwd sprintf('/SolMan Figures/%s figs/%s_session2_transfer_%s.fig', dirName, id, figure_file_suffix)]);
    end
    
end

%%%write cost data to csv file
TNC_Data = table2cell(TNC_Data); % convert from table to cell array 

t = readtable('TNC_Data.xlsx', 'ReadVariableNames',false);
rows = height(t); % find last used row in existing Excel doc
x = rows + 1; % find first empty row

xlswrite('TNC_Data.xlsx', TNC_Data, sprintf('A%d:K%d', x, x)); % append data to first empty row

close all;

end

function newFile = newParticipant(dirName, id, session)
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%% to read in participant files and concatenate each data for each session separately

%%% Function Inputs:
%%% fileName = parent file for data: 'NU', 'TD', or 'CP')
%%% id = participant id
%%% session = 1, 2, or 3 (original 200 trials from Session 1, retention, transfer)
%%% session = 4 is real life, but we haven't gotten there yet
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%%% file path on grey laptop
addpath(fullfile('C:\', 'Users', 'regamevrlab', 'Documents', 'MATLAB', 'Skittle', 'Codes to use', 'Data', dirName)); % add new path so we can load data from different directory
dataFolder = fullfile('C:\', 'Users', 'regamevrlab', 'Documents', 'MATLAB', 'Skittle', 'Codes to use', 'Data', dirName); % navigate to data file

%%% file path on Emily computer
% addpath(fullfile('C:\', 'Users', 'Rehabilitation Games', 'Desktop', 'Data', dirName)); % add new path so we can load data from different directory
% dataFolder = fullfile('C:\', 'Users',  'Rehabilitation Games', 'Desktop', 'Data', dirName); % navigate to data file

if session == 1
    d = dir(fullfile(dataFolder, sprintf('%s_1*_releases.mat', id)));
    
elseif session == 2
    d = dir(fullfile(dataFolder, sprintf('%s_2*_releases.mat', id)));
    
    %%% transfer data originally ended with transfer_releases, so had to remove
    %%% from retention search
    %%%% transfer data now ends with transferReleases to avoid this step!
    %     transfersToRemove = [];
    %
    %     for i = 1:length(d)
    %         transfersToRemove = [transfersToRemove; contains(d(i).name, 'transfer')];
    %     end
    %
    %     removeIndices = find(transfersToRemove);
    %     d(removeIndices) = [];
    
elseif session == 3
    d = dir(fullfile(dataFolder, sprintf('%s_2*_transferReleases.mat', id)));
end

filesToConcatenate = [];
for i=1:length(d)
    filesToConcatenate = [filesToConcatenate; load(d(i).name)];
end

for i=1:length(d)
    filesToConcatenate(i).releaseData = filesToConcatenate(i).releaseData(:,1:4);
end

releaseData = vertcat(filesToConcatenate.releaseData);

if session == 1
    newFile = sprintf('%s_session1.mat', id);
elseif session == 2
    newFile = sprintf('%s_session2_retention.mat', id);
elseif session == 3
    newFile = sprintf('%s_session2_transfer.mat', id);
end

%%% save concatenated version of data to analyze!
save(fullfile(dirName, newFile), 'releaseData');

end