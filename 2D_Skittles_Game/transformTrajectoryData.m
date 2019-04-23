function transformTrajectoryData(trialName)
fid = fopen(sprintf('%s.bin',trialName),'r');
[data,~]= fread(fid,[14,inf], 'double'); % error here for transfers 
for i = 1:length(data)
    angle(i) = gc2dec(data(3:14,i)')./4095*360; 
    if i>20
        velocity(i) = getVelocity(angle(i-19:i));
    else 
        velocity(i) = 0;
    end
end
time = data(1,:);
sensor = data(2,:);
fclose(fid);
save(fullfile('Bin transforms', trialName), 'time', 'angle', 'velocity', 'sensor');
end

function slope = getVelocity(angle)
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

 
