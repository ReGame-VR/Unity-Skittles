%% Virtual Skittle w/Obstaclescode by Zhaoran Zhang
% Modified by Johanna Dolleans and Emily Chicklis
%code to force exploration by creating obstacle in case of repetitve
%behavoir

function skittles_obstacles_for_exploration(name,visit,session,hand,i)
%% Parameter

trialsPerBlock = 25; % number of trials
thresholdO = .05; %threshold for Obstacles
thresholda = 15; %threshold for angle
thresholdv = 30; %threshold for velocity
threshold = .05; %threshold for target
%% Information regarding the Session
block = i;
subject = name;
Score = 0;
EScore = 0;
Obstacles1 = [];
ObsHit = [];  

if session >1 %gathering data from previous "session" (from last time code was run for participant)
    Calling=strcat(subject,'_',num2str(visit),'_',num2str(session-1),'_releases.mat');
    load(Calling)
    Score=releaseData(end,5);
    EScore = releaseData(end,6);
    trialNum = releaseData(end,7) + 1;
end

%% Information regarding the target and the figure
if hand == 'R'
    xtarget = .38;
else
    xtarget = -.38 ;
end
ytarget=.43;
target=[xtarget, ytarget];
graphicsScale = .25;
f=0;
k=f;
close all;

if session > 1
    Calling=strcat(subject,'_', num2str(visit), '_', num2str(session-1),'_','Obstacles');
    load(Calling);
    %         Obstacles1=Obstaclex;
end

f=size(Obstacles1,1);

if f >= 1
    fig = createUI2(target,graphicsScale,Obstacles1);
else
    fig = createUI(target,graphicsScale);
end
handles = guidata(fig);

%% Data Acquisition
s = daq.createSession('ni');
ai=s.addAnalogInputChannel('Dev2','ai0','Voltage');
ai.TerminalConfig='SingleEnded';
di=s.addDigitalChannel('Dev2','port0/line11:0','InputOnly');
s.Rate = 1000;
s.IsContinuous = true;
s.IsNotifyWhenDataAvailableExceedsAuto=false;
s.NotifyWhenDataAvailableExceeds=50;
setappdata(fig, 'encoder', zeros(1,12));

fid = fopen(sprintf('%s_%d_%d.bin', subject, visit, session),'w');
lh = s.addlistener('DataAvailable', @(src, event)getData(src, event, fid));
s.startBackground();

trial = 1;

R=[];
T=[];
setappdata(fig, 'simStep', 'waitToStart');
releaseData=zeros(trialsPerBlock,9); %9 cols rather than 7 to hold postHits and obsHits, respectively
Obsx=0;
Obsy=0;
Obstaclex=[];

while s.IsRunning
    angle = gc2dec(getappdata(fig, 'encoder'))./4095*360;
    set(handles.arm, 'XData', [0 .4*-cosd(angle)]*graphicsScale, 'YData', [-1.5 -1.5+.4*sind(angle)]*graphicsScale);
    switch(getappdata(fig, 'simStep'))
        case 'waitToStart'
        case 'waitToThrow'
            set(handles.trajectory,'Visible', 'off');
            set(handles.target, 'FaceColor', 'y');
            set(handles.ball, 'Position', [(.4*-cosd(angle))-.05,(-1.5+.4*sind(angle))-.05,.05*2,.05*2]*graphicsScale);
        case 'ballReleased'
            minDistance = 100;
            postHit = false;
            targetHit = false;
            spring = getappdata(fig, 'springParameters');
            tStart = getappdata(fig, 'drawTimeStart');
            time = toc(tStart);
            prevTime = time;
            i=1;
            while(time < 1.8 && postHit == false)
                time = toc(tStart);
                for t=prevTime:.001:time
                    checkx=spring(1).*sin(spring(5).*t+spring(3)).*exp(-t/spring(6));
                    checky=spring(2).*sin(spring(5).*t+spring(4)).*exp(-t/spring(6));
                    if (sqrt((checkx-target(1))^2 + (checky-target(2))^2) < minDistance)
                        minDistance = sqrt((checkx-target(1))^2 + (checky-target(2))^2);
                        if minDistance < threshold
                            targetHit = true;
                            set(handles.target, 'FaceColor', 'g');
                            postHit= true;
                            R=[R;getappdata(fig, 'release')];
                        end
                    end
                    if(checkx^2+checky^2 < .25^2)
                        postHit = true;
                        minDistance = 1;
                        releaseData(trial, 8) = 1; %save separate count of postHits to distinguish from obstacle hits
                    else
                        releaseData(trial, 8) = 0; 
                                       
                    end
                    
%                     disp("trial"); 
%                     disp(trial); 
%                     disp("initial Obshit"); 
%                     disp(ObsHit); 
%                     if f > 0
%                     %[m,n] = size(Obstacles1);  
%                     %if m > 0
%                         for o=1:f
%                         %for o = 1:m
%                             disp("o"); 
%                             disp(o); 
%                             if(sqrt((checkx-Obstacles1(o,2))^2+(checky-Obstacles1(o,3))^2) <thresholdO) 
%                                 disp("f");
%                                 disp(f); 
%                                 ObsHit(o)=ObsHit(o)+1;
%                                 disp("obshit"); 
%                                 disp(ObsHit); 
%                                 minDistance = 1;
%                                 releaseData(trial, 9) = 1; 
%                                 postHit = true;
%                             else
%                                 releaseData(trial, 9) = 0; %save obsHits separately for each trial
%                             end
%                         end
                end
 
%%%check for obstacle hits OUTSIDE of time for loop  
                    if f > 0
                    %[m,n] = size(Obstacles1);  
                    %if m > 0
                        for o=1:f
                        %for o = 1:m
                            if(sqrt((checkx-Obstacles1(o,2))^2+(checky-Obstacles1(o,3))^2) <thresholdO) 
                                ObsHit(o)=ObsHit(o)+1;
                                minDistance = 1;
                                releaseData(trial, 9) = 1; 
                                postHit = true;
                            else
                                releaseData(trial, 9) = 0; %save obsHits separately for each trial
                            end
                        end
            
                end
                x(i)=spring(1).*sin(spring(5).*time+spring(3)).*exp(-time/spring(6));
                y(i)=spring(2).*sin(spring(5).*time+spring(4)).*exp(-time/spring(6));
                set(handles.trajectory, 'XData', x*graphicsScale, 'YData', y*graphicsScale, 'Visible', 'on');
                set(handles.ball, 'Position', [x(i)-.05,y(i)-.05,.05*2,.05*2]*graphicsScale);
                angle = gc2dec(getappdata(fig, 'encoder'))./4095*360;
                set(handles.arm, 'XData', [0 .4*-cosd(angle)]*graphicsScale, 'YData', [-1.5 -1.5+.4*sind(angle)]*graphicsScale);
                drawnow;
                prevTime = time;
                i=i+1;
            end
            
            if targetHit == true
                T=[T;trial]; %save trial number at target hits
            end
            
            releaseData(trial,1:4) = [getappdata(fig, 'release') minDistance];
            
            if releaseData(trial,4)<.05
                Score=Score+10;
                ms1 = msgbox(sprintf('Congratulations, you hit the target! \n Your score is %d!', Score));     %create msgbox
                th = findall(ms1, 'Type', 'Text');                   %get handle to text within msgbox
                th.FontSize = 20;
                set(ms1, 'position', [1060 350 350 75]); %x position(?), y position, width, height
                
            end
            releaseData(trial,5) = Score; %save score for each trial
            
            if session > 1 && trial == 1
                if EScore > 1 
                ms2 = msgbox(sprintf('Your Exploration score from the \n previous session is %d!', EScore), 'test');
                    th = findall(ms2, 'Type', 'Text');                   %get handle to text within msgbox
                    th.FontSize = 20;
                    set(ms2, 'position', [1060 450 350 75]); %x position(?), y position, width, height
            
                end 
            end 
            
            %want to enourage exploration EVERY THROW - not just for hits!
            a=0; v=0;
            if (trial>4)
                
                for i=trial-4:trial-1
                    if abs(releaseData(i,2)-releaseData(trial,2))<thresholda
                        a=a+1;
                    end
                    if abs(releaseData(i,3)-releaseData(trial,3))<thresholdv
                        v=v+1;
                    end
                end
                
                if a<2
                    EScore=EScore+5;
                end
                
                if v<2
                    EScore=EScore+5;
                end
                
                if EScore > releaseData(trial-1,6) %only display exploration score if it increased
                    ms2 = msgbox(sprintf('Your Exploration score is %d!', EScore), 'test');
                    th = findall(ms2, 'Type', 'Text');                   %get handle to text within msgbox
                    th.FontSize = 20;
                    set(ms2, 'position', [1060 450 350 75]); %x position(?), y position, width, height
                end
                
                releaseData(trial,6) = EScore; %save EScore for each trial
                
                meanA=0; meanV=0;
                if releaseData(trial,4)<.05 %if target hit
                    a=0; v=0;
                    indice=find(T==trial);%search through trials of previous hits
                    if indice>4
                        for i=indice-4:indice-1 %check previous four angles and velocities
                            if abs(releaseData(T(i),2)-releaseData(trial,2))<thresholda
                                a=a+1; %increment a when angle less than diff threshold
                            end
                            if abs(releaseData(T(i),3)-releaseData(trial,3))<thresholdv
                                v=v+1; %increment v when velocity less than diff threshold
                            end
                        end
                        if v > 2
                            meanV=releaseData(trial,3);
                        end
                        if a > 2
                            meanA=releaseData(trial,2);
                        end
                    end
                    
                    if abs(meanA)>0
                        if abs(meanV)>0
                            spring=getSpringParameters(meanA, meanV);
                            time=linspace(0.01,2,200);
                            x2=spring(1).*sin(spring(5).*time+spring(3)).*exp(time./spring(6));
                            y2=spring(2).*sin(spring(5).*time+spring(4)).*exp(time./spring(6));
                            indice=find(abs(y2)<0.05);
                            Obsx=x2(indice(1));
                            f=f+1;
                            k=k+1;
                            ObsHit(f)=0;
                            Obstaclex(k,1)=trial+trial*(block-1);
                            Obstaclex(k,2)=Obsx;
                            Obstaclex(k,3)=Obsy;
                            Obstaclex(k,4)=a;
                            Obstaclex(k,5)=v;
                            
                            Obstacles1=[Obstacles1;Obstaclex(k,:)];
                            clear meanA meanV Obsx x2 y2 T
                            T=[];
                            
                            for x = 1:size(Obstacles1, 1)
                                switch x
                                    case 1
                                        set(handles.obstacle,'Visible', 'on');
                                        set(handles.obstacle, 'Position', [Obstacles1(1,2)-.05,Obstacles1(1,3)-.05,.05*2,.05*2]*graphicsScale);
                                    case 2
                                        set(handles.obstacle2,'Visible', 'on');
                                        set(handles.obstacle2, 'Position', [Obstacles1(2,2)-.05,Obstacles1(2,3)-.05,.05*2,.05*2]*graphicsScale);
                                    case 3
                                        set(handles.obstacle3,'Visible', 'on');
                                        set(handles.obstacle3, 'Position', [Obstacles1(3,2)-.05,Obstacles1(3,3)-.05,.05*2,.05*2]*graphicsScale);
                                   case 4
                                        set(handles.obstacle4,'Visible', 'on');
                                        set(handles.obstacle4, 'Position', [Obstacles1(4,2)-.05,Obstacles1(4,3)-.05,.05*2,.05*2]*graphicsScale);
                                   case 5
                                        set(handles.obstacle5,'Visible', 'on');
                                        set(handles.obstacle5, 'Position', [Obstacles1(5,2)-.05,Obstacles1(5,3)-.05,.05*2,.05*2]*graphicsScale);
                                end
                            end
                            
                            %                             switch k
                            %                                 case 1
                            %                                     set(handles.obstacle,'Visible', 'on');
                            %                                     set(handles.obstacle, 'Position', [Obstaclex(1,2)-.05,Obstaclex(1,3)-.05,.05*2,.05*2]*graphicsScale);
                            %                                 case 2
                            %                                     set(handles.obstacle2,'Visible', 'on');
                            %                                     set(handles.obstacle2, 'Position', [Obstaclex(2,2)-.05,Obstaclex(2,3)-.05,.05*2,.05*2]*graphicsScale);
                            %                                 case 3
                            %                                     set(handles.obstacle3,'Visible', 'on');
                            %                                     set(handles.obstacle3, 'Position', [Obstaclex(3,2)-.05,Obstaclex(3,3)-.05,.05*2,.05*2]*graphicsScale);
                            %                             end
                        end
                    end
                end
            end
            
            if session > 1
                releaseData(trial,7) = trialNum;
            else
                releaseData(trial, 7) = trial;
            end
            
            if trial == trialsPerBlock
                s.stop();
            else
                trial = trial+1;
                if session > 1
                    trialNum = trialNum+1;
                end
                setappdata(fig, 'simStep', 'waitToStart');
                clear x y;
            end
    end
    drawnow;
    
end
%% end of acquisition and saving of data
delete(lh);
fclose(fid);

trialName=strcat(subject,'_',num2str(visit),'_',num2str(session));
save(sprintf('%s_%d_%d_releases.mat', subject, visit, session), 'releaseData');
save(sprintf('%s_%d_%d_Obstacles.mat', subject,visit, session), 'Obstacles1','ObsHit');

close all;
close all hidden; %closes all message boxes

transformTrajectoryData(trialName); %transform bin file to .mat format
fclose('all');
movefile(sprintf('%s.bin',trialName), fullfile('Bin files')); %%% move bin file to bin folder
end
%% Function to create the figure of the virtual Skitlle
function fig = createUI(target,graphicsScale)
fig = figure('Tag','fig',...
    'Units','normalized',...
    'OuterPosition',[1, 0, 1, 1],...
    'Color',[0 0 0],...
    'Renderer', 'zbuffer', ...
    'MenuBar', 'none', ...
    'ToolBar', 'none', ...
    'Resize', 'off');

ax = axes('Parent',fig,...
    'Tag','ax', ...
    'Units','normalized',...
    'Position',[0 0 1 1],...
    'XLim',[-.6 .6],...
    'YLim',[-.51 .51],...
    'XLimMode', 'manual', ...
    'YLimMode', 'manual', ...
    'Color',[0 0 0]);

daspect('manual');

post = rectangle('Parent',ax,...
    'Tag','post',....
    'Position',[0-.25,0-.25,.25*2,.25*2]*graphicsScale,...
    'LineStyle', 'none', ...
    'Curvature',[1,1], ...
    'FaceColor',[1 0 0]);

target = rectangle('Parent',ax,...
    'Tag','target',....
    'Position',[target(1)-.05,target(2)-.05,.05*2,.05*2]*graphicsScale,...
    'LineStyle', 'none', ...,
    'Curvature',[1,1], ...
    'FaceColor',[1 1 0]);

armJoint = rectangle('Parent',ax,...
    'Tag','armJoint',....
    'Position',[0-.02,-1.5-.02,.01*2,.02*2]*graphicsScale,...
    'LineStyle', 'none', ...
    'Curvature',[1,1], ...
    'FaceColor',[1 1 1]);

arm = line('Parent',ax, ...
    'Tag','arm', ...
    'LineStyle', '-',...
    'XData', [0.4 0]*graphicsScale,...
    'YData', [-1.5 -1.5]*graphicsScale,...
    'LineStyle', '-', ...
    'LineWidth', 3, ...
    'Color',[1 0 1]);

trajectory = line('Parent',ax, ...
    'Tag','trajectory', .....
    'XData', [-10 -10]*graphicsScale,...
    'YData', [-10 -10]*graphicsScale,...
    'LineStyle', '-', ...
    'LineWidth', 2, ...
    'Color','w', ...
    'Visible', 'off');

ball = rectangle('Parent',ax,...
    'Tag','ball',....
    'LineStyle', 'none', ...
    'Position', [-10,-10,.05*2,.05*2]*graphicsScale, ...
    'Curvature',[1,1], ...
    'FaceColor',[1 1 1], ...
    'Visible', 'off');

obstacle = rectangle('Parent',ax,...
    'Tag','obstacle',....
    'LineStyle', 'none', ...
    'Position', [-5,0,.05*2,.05*2]*graphicsScale, ...
    'Curvature',[1,1], ...
    'FaceColor',[0 0 1], ...
    'Visible', 'off');

obstacle2 = rectangle('Parent',ax,...
    'Tag','obstacle2',....
    'LineStyle', 'none', ...
    'Position', [-5,0,.05*2,.05*2]*graphicsScale, ...
    'Curvature',[1,1], ...
    'FaceColor',[0 0 1], ...
    'Visible', 'off');
obstacle3 = rectangle('Parent',ax,...
    'Tag','obstacle3',....
    'LineStyle',  'none', ...
    'Position', [-5,0,.05*2,.05*2]*graphicsScale, ...
    'Curvature',[1,1], ...
    'FaceColor',[0 0 1], ...
    'Visible', 'off');
% create handles structure
ad = guihandles(fig);

% save application data
guidata(fig, ad);
end
function fig = createUI2(target,graphicsScale,Obstacle1)
fig = figure('Tag','fig',...
    'Units','normalized',...
    'OuterPosition',[1, 0, 1, 1],...
    'Color',[0 0 0],...
    'Renderer', 'zbuffer', ...
    'MenuBar', 'none', ...
    'ToolBar', 'none', ...
    'Resize', 'off');

ax = axes('Parent',fig,...
    'Tag','ax', ...
    'Units','normalized',...
    'Position',[0 0 1 1],...
    'XLim',[-.6 .6],...
    'YLim',[-.51 .51],...
    'XLimMode', 'manual', ...
    'YLimMode', 'manual', ...
    'Color',[0 0 0]);

daspect('manual');

post = rectangle('Parent',ax,...
    'Tag','post',....
    'Position',[0-.25,0-.25,.25*2,.25*2]*graphicsScale,...
    'LineStyle', 'none', ...
    'Curvature',[1,1], ...
    'FaceColor',[1 0 0]);

target = rectangle('Parent',ax,...
    'Tag','target',....
    'Position',[target(1)-.05,target(2)-.05,.05*2,.05*2]*graphicsScale,...
    'LineStyle', 'none', ...,
    'Curvature',[1,1], ...
    'FaceColor',[1 1 0]);

armJoint = rectangle('Parent',ax,...
    'Tag','armJoint',....
    'Position',[0-.02,-1.5-.02,.01*2,.02*2]*graphicsScale,...
    'LineStyle', 'none', ...
    'Curvature',[1,1], ...
    'FaceColor',[1 1 1]);

arm = line('Parent',ax, ...
    'Tag','arm', ...
    'LineStyle', '-',...
    'XData', [0.4 0]*graphicsScale,...
    'YData', [-1.5 -1.5]*graphicsScale,...
    'LineStyle', '-', ...
    'LineWidth', 3, ...
    'Color',[1 0 1]);

trajectory = line('Parent',ax, ...
    'Tag','trajectory', .....
    'XData', [-10 -10]*graphicsScale,...
    'YData', [-10 -10]*graphicsScale,...
    'LineStyle', '-', ...
    'LineWidth', 2, ...
    'Color','w', ...
    'Visible', 'off');

ball = rectangle('Parent',ax,...
    'Tag','ball',....
    'LineStyle', 'none', ...
    'Position', [-10,-10,.05*2,.05*2]*graphicsScale, ...
    'Curvature',[1,1], ...
    'FaceColor',[1 1 1], ...
    'Visible', 'off');

obstacle = rectangle('Parent',ax,...
    'Tag','obstacle',....
    'LineStyle', 'none', ...
    'Position', [-5,0,.05*2,.05*2]*graphicsScale, ...
    'Curvature',[1,1], ...
    'FaceColor',[0 0 1], ...
    'Visible', 'off');

obstacle2 = rectangle('Parent',ax,...
    'Tag','obstacle2',....
    'LineStyle', 'none', ...
    'Position', [-5,0,.05*2,.05*2]*graphicsScale, ...
    'Curvature',[1,1], ...
    'FaceColor',[0 0 1], ...
    'Visible', 'off');

obstacle3 = rectangle('Parent',ax,...
    'Tag','obstacle3',....
    'LineStyle', 'none', ...
    'Position', [-5,0,.05*2,.05*2]*graphicsScale, ...
    'Curvature',[1,1], ...
    'FaceColor',[0 0 1], ...
    'Visible', 'off');

obstacle3 = rectangle('Parent',ax,...
    'Tag','obstacle3',....
    'LineStyle', 'none', ...
    'Position', [-5,0,.05*2,.05*2]*graphicsScale, ...
    'Curvature',[1,1], ...
    'FaceColor',[0 0 1], ...
    'Visible', 'off');

obstacle4 = rectangle('Parent',ax,...
    'Tag','obstacle4',....
    'LineStyle', 'none', ...
    'Position', [-5,0,.05*2,.05*2]*graphicsScale, ...
    'Curvature',[1,1], ...
    'FaceColor',[0 0 1], ...
    'Visible', 'off');

obstacle5 = rectangle('Parent',ax,...
    'Tag','obstacle5',....
    'LineStyle', 'none', ...
    'Position', [-5,0,.05*2,.05*2]*graphicsScale, ...
    'Curvature',[1,1], ...
    'FaceColor',[0 0 1], ...
    'Visible', 'off');

if size(Obstacle1,1)>4
    obstacle15 = rectangle('Parent',ax,...
        'Tag','obstacle5',....
        'LineStyle', 'none', ...
        'Position', [Obstacle1(5,2)-.05,Obstacle1(5,3)-.05,.05*2,.05*2]*graphicsScale, ...
        'Curvature',[1,1], ...
        'FaceColor',[0 0 1], ...
        'Visible', 'on');
end

if size(Obstacle1,1)>3
    obstacle14 = rectangle('Parent',ax,...
        'Tag','obstacle4',....
        'LineStyle', 'none', ...
        'Position', [Obstacle1(4,2)-.05,Obstacle1(4,3)-.05,.05*2,.05*2]*graphicsScale, ...
        'Curvature',[1,1], ...
        'FaceColor',[0 0 1], ...
        'Visible', 'on');
end

if size(Obstacle1,1)>2
    obstacle13 = rectangle('Parent',ax,...
        'Tag','obstacle3',....
        'LineStyle', 'none', ...
        'Position', [Obstacle1(3,2)-.05,Obstacle1(3,3)-.05,.05*2,.05*2]*graphicsScale, ...
        'Curvature',[1,1], ...
        'FaceColor',[0 0 1], ...
        'Visible', 'on');
end

if size(Obstacle1,1)>1
    obstacle12 = rectangle('Parent',ax,...
        'Tag','obstacle2',....
        'LineStyle', 'none', ...
        'Position', [Obstacle1(2,2)-.05,Obstacle1(2,3)-.05,.05*2,.05*2]*graphicsScale, ...
        'Curvature',[1,1], ...
        'FaceColor',[0 0 1], ...
        'Visible', 'on');
end

obstacle11 = rectangle('Parent',ax,...
    'Tag','obstacle',....
    'LineStyle', 'none', ...
    'Position', [Obstacle1(1,2)-.05,Obstacle1(1,3)-.05,.05*2,.05*2]*graphicsScale, ...
    'Curvature',[1,1], ...
    'FaceColor',[0 0 1], ...
    'Visible', 'on');

% create handles structure
ad = guihandles(fig);

% save application data
guidata(fig, ad);
end

%% functions to get the data and calculate the velocity and trajectory
function getData(src, event, fid)
fig = findall(0, 'Tag', 'fig');
setappdata(fig, 'encoder', event.Data(end, 2:13))
%setappdata(fig2,'encoder2',event2.Data(end,2:13));
data = [event.TimeStamps, event.Data]';
%data2=[event2.TimeStamps,event2.Data];
fwrite(fid,data,'double');
switch(getappdata(fig, 'simStep'))
    case 'waitToStart'
        if any(event.Data(:,1) > 2.0)%&&(event2.Data(:,1) > 2.0)
            setappdata(fig,'simStep', 'waitToThrow');
            handles = guidata(fig);
            set(handles.ball, 'Visible', 'on');
        end
    case 'waitToThrow'
        if any(event.Data(:,1) < 1)%&&(event2.Data(:,1) < 0.5)
            setappdata(fig,'simStep', 'ballReleased');
            releaseIndex = find(event.Data(:,1) < 1.0, 1);
            releaseAngle =  gc2dec(event.Data(releaseIndex, 2:13))./4095*360;
            
            if releaseIndex < 20
                pastAngles = getappdata(fig, 'pastAngles');
                releaseVelocity = getVelocity([pastAngles(end-(19-releaseIndex):end, :); event.Data(1:releaseIndex, 2:13)]);
            else
                releaseVelocity = getVelocity(event.Data(releaseIndex-19:releaseIndex, 2:13));
            end
            setappdata(fig, 'release', [double(src.ScansAcquired-50+releaseIndex), releaseAngle, releaseVelocity]);
            setappdata(fig, 'drawTimeStart', tic);
            spring = getSpringParameters(releaseAngle, releaseVelocity);
            setappdata(fig, 'springParameters', spring);
        end
        setappdata(fig,'pastAngles', event.Data(end-18:end, 2:13));
    case 'ballReleased'
end
end

function slope = getVelocity(data)
angle = zeros(1,20);
for i = 1:20
    angle(i) = gc2dec(data(i,:))./4095*360;
end
boundary = find(abs(diff(angle))>300);
if ~isempty(boundary)
    if angle(boundary+1)-angle(boundary) > 0
        angle(boundary+1:end) = angle(boundary+1:end)-360;
    else
        angle(1:boundary) = angle(1:boundary)-360;
    end
end
time = 0:.001:.019;
slope = sum((time-mean(time)).*(angle-mean(angle)))/sum((time-mean(time)).*(time-mean(time)));

end

function spring = getSpringParameters(releaseAngle, releaseVelocity)
m = 0.1; %mass
k = 1; %spring constant
c = 0.01;   %viscous damping
T = (2*m/c);    %relaxation time
%L=0.95;
%g=9.81;
%T=2*m*g/L;
%w=sqrt(g)/L;
w = sqrt(abs((k/m)-((1/T)^2))); %frequency
l = 0.4; %arm length

x0=-l*cosd(releaseAngle);
y0=-1.5+l*sind(releaseAngle);
vx0=(releaseVelocity*pi/180*l)*sind(releaseAngle);
vy0=(releaseVelocity*pi/180*l)*cosd(releaseAngle);

ax=sqrt((x0)^2+((vx0/w)+(x0/T)/w)^2);
ay=sqrt((y0)^2+((vy0/w)+(y0/T)/w)^2);

if ax~=0
    px=acos((1/ax)*((vx0/w)+((x0/T)/w)));
else
    px=0;
end
if x0<0
    px=-px;
end

if ay~=0
    py=acos((1/ay)*((vy0/w)+((y0/T)/w)));
else py=0;
end
if y0<0
    py=-py;
end
spring = [ax,ay,px,py,w,T];
end
function dec = gc2dec(gra)
s2 = size(gra,2);
bin = char(zeros(1,s2));

for j2 = s2:-1:2
    if mod(sum(gra(1:j2-1)),2) == 1
        bin(j2) = int2str(1 - gra(j2));
    else
        bin(j2) = int2str(gra(j2));
    end
end

bin(1) = int2str(gra(1));
dec = bin2dec(bin);
end