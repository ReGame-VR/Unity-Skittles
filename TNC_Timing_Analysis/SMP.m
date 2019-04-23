function [mindist,mindistPoint,hitPost] = SMP(a,v,xtarget,ytarget)
% both a v are deg

    m=0.068; %mass
    k=.67; %spring constant
    c=0.01;   %viscus damping
    flytime=1.6;
    Post=[0,0];

    

hitPost=0;
T=(2*m/c);    %relaxation time
w=sqrt(abs((k/m)-((1/T)^2))); %frequency
rc=0.125; %radius of center post
xp0=-.1;   %rotation point of paddle
yp0=-.65;
l=0.3; %length of arm

a=a*pi/180;
v=v*pi/180;
xr=xp0-l*cos(a);
yr=yp0+l*sin(a);
vx0=(v*l)*sin(a);
vy0=(v*l)*cos(a);
Ax=sqrt((xr)^2+((vx0/w)+(xr/T)/w)^2);
Ay=sqrt((yr)^2+((vy0/w)+(yr/T)/w)^2);
if Ax~=0
    u=(1/Ax)*((vx0/w)+((xr/T)/w));
    phasex=acos(u);
else
    phasex=0;
end

if xr<0
    phasex=-phasex;
end

if Ay~=0
    u=(1/Ay)*((vy0/w)+((yr/T)/w));
    phasey=acos(u);
else
    phasey=0;
end
if yr<0
    phasey=-phasey;

end

time = linspace(0,flytime,flytime*1000+1);
xtime=Ax.*sin(w.*time+phasex).*exp(-time/T);
ytime=Ay.*sin(w.*time+phasey).*exp(-time/T);
xydistCP = sqrt((xtime-Post(1)).^2+(ytime-Post(2)).^2);
xydist = sqrt((xtime-xtarget).^2+(ytime-ytarget).^2);

if ~isempty(find(xydistCP<rc))
    % cut trajectory after find(xydisCP<rc,1)
    xtime_res=xtime(1:find(xydistCP<rc,1));
    ytime_res=ytime(1:find(xydistCP<rc,1));% residual trajectory
    xydist_res = sqrt((xtime_res-xtarget).^2+(ytime_res-ytarget).^2);
    Dis_post=sqrt((xtime(find(xydistCP<rc,1))-xtarget)^2+(ytime(find(xydistCP<rc,1))-ytarget)^2); % distance between post hit point to target
    Dis_traj=min(xydist_res);   
    if Dis_post<=Dis_traj
        hitPost=1;
        mindistTime=find(xydistCP<rc,1);
        mindistPoint=[xtime(mindistTime),ytime(mindistTime)];
        %mindist = xydist(mindistTime)*(-1);
        mindist = 1;
    else            
        mindistTime=find(xydist == Dis_traj);
        mindistPoint=[xtime(mindistTime),ytime(mindistTime)];
        %mindist = xydist(mindistTime)*sign(sqrt(mindistPoint(1)^2+mindistPoint(2)^2)-sqrt(xtarget^2+ytarget^2));
        mindist = xydist(mindistTime);
        if sqrt(mindistPoint(1)^2+mindistPoint(2)^2)==sqrt(xtarget^2+ytarget^2)
            mindist = abs(xydist(mindistTime));
        end
    end
else
    mindistTime = find(xydist == min(xydist));
    if mindistTime==length(xydist)
        mindist=xydist(end);
        mindistPoint=[xtime(end),ytime(end)];
    else
    mindistPoint=[xtime(mindistTime),ytime(mindistTime)];
    %mindist = xydist(mindistTime)*sign(sqrt(mindistPoint(1)^2+mindistPoint(2)^2)-sqrt(xtarget^2+ytarget^2));
    mindist = xydist(mindistTime);
    if sqrt(mindistPoint(1)^2+mindistPoint(2)^2)==sqrt(xtarget^2+ytarget^2)
        mindist = abs(xydist(mindistTime));
    end

    end
end
end
