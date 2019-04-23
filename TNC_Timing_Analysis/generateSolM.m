function D = generateSolM(target, session, hand, counter, already_computed)
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% generateSM.m
% script to genarate the solution manifold as a matrix

% Rajal Cohen, Action Lab, Kinesiology Dept,Pennsylvania State University
% Modified by Se-Woong Park, Northeastern University
% function to compute and save the solution manifold for a given target

% calls execution2result_polar_regular (originally execution2result_polar) 

%%% function called by RunTNC2D execution code

%%% counter: 1 = abs val velocities, 2 = positive velocities, 3 = negative velocities
%%% already_computed = 1 if you already have the SM; 0 if you don't
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

xtarget = target(1,1);
ytarget = target(1,2);

%%%grain should be 10x the range of the angles + 1 - so if range of angles is 0 to 180, use 1801
grain = 1801; 

%%% load solution manifold below if you already have it
if already_computed == 1
    
    if counter == 1 || counter == 2  % positive velocities
        if session == 1 || session == 2
            fileName = sprintf('C:/Users/regamevrlab/Documents/MATLAB/Skittle/Codes to use/Analysis/2D/SolMan Figures/SM_pos_%s_original.mat', hand);
        else
            fileName = sprintf('C:/Users/regamevrlab/Documents/MATLAB/Skittle/Codes to use/Analysis/2D/SolMan Figures/SM_pos_%s_transfer.mat', hand);
        end
    else
        % negative velocities
        if session == 1 || session == 2
            fileName = sprintf('C:/Users/regamevrlab/Documents/MATLAB/Skittle/Codes to use/Analysis/2D/SolMan Figures/SM_neg_%s_original.mat', hand);
        else
            fileName = sprintf('C:/Users/regamevrlab/Documents/MATLAB/Skittle/Codes to use/Analysis/2D/SolMan Figures/SM_neg_%s_transfer.mat', hand);
        end
    end
    
end

%%% adjust angle and velocity linspace to create range of the entire figure

% Create vectors of angles and velocities in upper left quadrant
%angle = linspace(-180, 180, grain); % units = degrees
%angle = linspace(0, 180, grain); % units = degrees
%velocity = linspace(-800, -200, grain); % units = degrees/sec

if counter == 1 || counter == 2
    %positive velocities
    angle = linspace(0, 180, grain); % units = degrees
    velocity = linspace(0, 1000, grain); % units = degrees/sec
else
    %negative velocities
    angle = linspace(0, 180, grain); % units = degrees
    velocity = linspace(-1000, 0, grain); % units = degrees/sec
end

%creates 2 matrices of size(angle)x size(velocity)
[X,Y]=meshgrid(angle,velocity);

if already_computed == 0
    
    disp('calculating distances')
    D = zeros(grain);
    
    for ang=1:grain %loop through many possible angles and velocities and test each
        % show progress
        if rem(ang,60) == 0
            disp(strcat('angle=',num2str(ang)));
        end
        % calculate the solution manifold
        for vel=1:grain
            % D is the solution manifold
            D(ang, vel) = execution2result_polar_regular(angle(ang),velocity(vel),xtarget,ytarget);
        end
    end
    %toc % end calculating distance
    
    % save SolMan matrix for future use
    if counter == 1 || counter == 2
        if session == 1 || session == 2
            save(sprintf('C:/Users/regamevrlab/Documents/MATLAB/Skittle/Codes to use/Analysis/2D/SolMan Figures/SM_pos_%s_original.mat', hand),'D')
        else
            save(sprintf('C:/Users/regamevrlab/Documents/MATLAB/Skittle/Codes to use/Analysis/2D/SolMan Figures/SM_pos_%s_transfer.mat', hand),'D')
        end
    else
        if session == 1 || session == 2
            save(sprintf('C:/Users/regamevrlab/Documents/MATLAB/Skittle/Codes to use/Analysis/2D/SolMan Figures/SM_neg_%s_original.mat', hand),'D')
        else
            save(sprintf('C:/Users/regamevrlab/Documents/MATLAB/Skittle/Codes to use/Analysis/2D/SolMan Figures/SM_neg_%s_transfer.mat', hand),'D')
        end
    end
    
else % manifold is already computed and just needs to be plotted
    
    load(fileName);
    
end

% the matrix is transposed for a visualization purpose
d = D';
% shrink X and Y to account for smoothing
X = X(1:grain,1:grain);
Y = Y(1:grain,1:grain);

figure %%%pop-up window of graph

%plot using a function 'pcolor'
pcolor(X,Y,d*100) %in centimeters
shading interp

xlabel('Angle (deg)')
ylabel('Velocity (deg/s)')

% create the colormap here
R=linspace(1,0,256).^2;
G=linspace(1,0,256).^5;
B=linspace(1,0,256).^7;
CLUT=[R' G' B'];

colormap(CLUT)

end
