%% Virtual Skittles w/Score: code by Zhaoran Zhang
% Modified by Johanna Dolleans
%code to force exploration by  creating a score in
%case of repetitve behavoir

function skittles_transfer(name, visit, session, hand, block)
%% Parameter
trialsPerBlock = 25; % number of trials
thresholda = 15; %threshold for angle
thresholdv = 30; %threshold for velocity
threshold =.05; %threshold for target

%% Information regarding the Session
subject = name;

%% Information regarding the target and the figure
if hand == 'R'
    xtarget = .2;
else
    xtarget = -.2;
end
ytarget = .5;
target = [xtarget, ytarget];
threshold = .05;
graphicsScale = .25;
Score=0;
EScore=0;
trial_track = 0;
close all;
fig = createUI(target,graphicsScale);
handles = guidata(fig);

%     if block >1
%     Calling=strcat(subject,'_',num2str(visit),'_',num2str(session),'_',num2str(block-1),'_releases.mat');
%     load(Calling)
%     Score=releaseData(end,5);
%     Escore= releaseData(end,6);
%     trial_track = releaseData(end, 7);
%
%         if session >1
%         Calling=strcat(subject,'_',num2str(visit),'_',num2str(session-1),'_',num2str(block-1),'_releases.mat');
%         load(Calling)
%         Score=releaseData(end,5);
%         Escore= releaseData(end,6);
%         trial_track = releaseData(end, 7);
%
%         end
%
%     else

    if session > 1
        Calling=strcat(subject,'_',num2str(visit),'_',num2str(session-1),'_transferReleases.mat');
        load(Calling)
        Score=releaseData(end,5);
        Escore= releaseData(end,6);
        trialNum = releaseData(end,7) + 1;

    end

%     end

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

%fid = fopen(sprintf('%s_%d_%d.trialNum', subject, session, block),'w'); 
%%% above line of code had been overwriting the first session 1 bin file
%%% and the first retention bin file 

fid = fopen(sprintf('%s_%d_%d_transfer.bin', subject, visit, session),'w'); 
lh = s.addlistener('DataAvailable', @(src, event)getData(src, event, fid));
s.startBackground();

trial = 1;
R = []; 
setappdata(fig, 'simStep', 'waitToStart');
releaseData=zeros(trialsPerBlock,7);

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
            %j=1;
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
                            %X(j)=spring(1).*sin(spring(5).*0.75+spring(3)).*exp(-t/spring(6));
                            %Y(j)=spring(2).*sin(spring(5).*0.75+spring(4)).*exp(-t/spring(6));
                        end
                    end
                    if(checkx^2+checky^2 < .25^2)
                        postHit = true;
                        minDistance = 1;
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
            
            releaseData(trial,1:4) = [getappdata(fig, 'release') minDistance];
            if releaseData(trial,4)<.05
                Score=Score+10;
                ms1 = msgbox(sprintf('Congratulations, you hit the target! \n Your score is %d!', Score), 'test');     %create msgbox
                th = findall(ms1, 'Type', 'Text');                   %get handle to text within msgbox
                th.FontSize = 20;
                set(ms1, 'position', [1060 350 350 75]); %makes box bigger
                %    set(ms1, 'position', [1100 75 50 50]); %makes box bigger
            end
            releaseData(trial,5) = Score;
        
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
trialName=strcat(subject,'_',num2str(visit),'_', num2str(session), '_transfer'); % same as name of bin file
save(sprintf('%s_%d_%d_transferReleases.mat', subject, visit, session), 'releaseData');

close all;
close all hidden; %close message boxes

transformTrajectoryData(trialName); %transform bin file to .mat format
fclose('all');
movefile(sprintf('%s.bin',trialName), fullfile('Bin files')); %%% move bin file to bin folder
end

function fig = createUI(target,graphicsScale)
% 'OuterPosition',[1, 0, 1, 1]
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
% create handles structure
ad = guihandles(fig);

% save application data
guidata(fig, ad);
end

function getData(src, event, fid)
fig = findall(0, 'Tag', 'fig');
setappdata(fig, 'encoder', event.Data(end, 2:13))
data = [event.TimeStamps, event.Data]';
fwrite(fid,data,'double');
switch(getappdata(fig, 'simStep'))
    case 'waitToStart'
        if any(event.Data(:,1) > 2.0)
            setappdata(fig,'simStep', 'waitToThrow');
            handles = guidata(fig);
            set(handles.ball, 'Visible', 'on');
        end
    case 'waitToThrow'
        if any(event.Data(:,1) < 1)
            setappdata(fig,'simStep', 'ballReleased');
            releaseIndex = find(event.Data(:,1) < 1, 1);
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
trialNum = char(zeros(1,s2));

for j2 = s2:-1:2
    if mod(sum(gra(1:j2-1)),2) == 1
        trialNum(j2) = int2str(1 - gra(j2));
    else
        trialNum(j2) = int2str(gra(j2));
    end
end

trialNum(1) = int2str(gra(1));
dec = bin2dec(trialNum);
end