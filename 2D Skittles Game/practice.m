%% Non redundnat virtual Skittle code by Zhaoran Zhang
% Modified by Johanna DOlleans

function practice(name,session,hand,block)

trialsPerBlock = 10;
threshold = .05;

%% Information regarding the Session
subject = name;
Score=0;
%% Information regarding the target and the figure
if hand == 'R'
    xtarget = .38;
else
    xtarget = -.38 ;
end
ytarget=.43;
target = [xtarget, ytarget];
graphicsScale = .25;
% Information regarding the non redundancy

close all;
fig = createUI(target,graphicsScale);
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

fid = fopen(sprintf('%s_practice_bin.bin', subject),'w');
lh = s.addlistener('DataAvailable', @(src, event)getData(src, event, fid));
s.startBackground();

trial = 1;
R=[];
setappdata(fig, 'simStep', 'waitToStart');
releaseData=zeros(trialsPerBlock,4);

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
            releaseData(trial,:) = [getappdata(fig, 'release') minDistance];
            %                 if releaseData(trial,4)<.05
            %        if trial > 1
            %           delete(ms1)
            %        end
            %        Score=Score+10;
            %        ms1 = msgbox(sprintf( 'Great Job! \n Your  score is %d !', Score), 'test');     %create msgbox
            % th = findall(ms1, 'Type', 'Text');                   %get handle to text within msgbox
            % th.FontSize = 20;
            %      set(ms1, 'position', [1060 350 350 75]); %makes box bigger
            % %        set(ms1, 'position', [1100 75 50 50]); %makes box bigger
            %  else
            %        ms1 = msgbox(sprintf( 'Nice try, keep up the good work!!'), 'test');     %create msgbox
            % th = findall(ms1, 'Type', 'Text');                   %get handle to text within msgbox
            % th.FontSize = 20;
            %      set(ms1, 'position', [1060 350 350 75]); %makes box bigger
            %    end
            if trial == trialsPerBlock
                s.stop();
            else
                trial = trial+1;
                setappdata(fig, 'simStep', 'waitToStart');
                clear x y;
            end
    end
    drawnow;
end
%% end of acquisition and saving of data
delete(lh);
fclose(fid);

trialName=strcat(subject,'_practice_bin');
save(sprintf('%s_%d_%d_practice.mat', subject, session, block), 'releaseData');

close all;

transformTrajectoryData(trialName); %transform bin file to .mat format
fclose('all');
movefile(sprintf('%s.bin',trialName), fullfile('Bin files')); %%% move bin file to bin folder

%% TNC Analysis of the data
%     Results=AnalysisTNC(trialName);
%     save(sprintf('Results_analysis_%s_%d_%d_.mat',subject, session, block),'Results');
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


% create handles structure
ad = guihandles(fig);

% save application data
guidata(fig, ad);
end
%% functions to get the data and calculate the velocity and trajectory
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
        if any(event.Data(:,1) < 1.2)
            setappdata(fig,'simStep', 'ballReleased');
            releaseIndex = find(event.Data(:,1) < 1.2, 1);
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