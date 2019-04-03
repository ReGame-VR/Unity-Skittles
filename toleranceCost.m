function [AVD, idealAVD, Tcost] = toleranceCost(VXY, AVD, D, counter)
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%when altering this code to match new data, check that centerRangeA and
%%%centerRangeV make sense for your data, and the same for velocity_index
%%%and angle_index
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% Tolerance cost calculation
% Programmed by the Action Lab (Penn State/Northeastern University)
% For questions, contact Se-Woong Park (s.park@neu.edu)
%
% Inputs
% VXY: coordinate system (0: polar, 1: Cartesian)
% AVD: N X 3 matrix (N: # of trials, 1st col: angles in degree, 2nd col:
%                    angular velocities in deg/s,3rd: Error)
%
% D: solution manifold that was aready made by generateSM.m
%
% Outputs
% AVD: Original data cloud
% IdealAVD: Ideal data cloud
% T_cost: T-cost

% (large enough) initial number
idealMean = 9999999;

% mean_a = mean(AVD(:,1));
% mean_v = mean(AVD(:,2));

center_a = median(AVD(:,1));
center_v = median(AVD(:,2));

% number of grids for each axis
gridA = 201;
gridV = 201;

%%%range of values here can be a bit smaller than those for the entire
%%%figure in generateSolM, so that they focus on the center of the actual manifold
centerRangeA = linspace(30,150,gridA);

%%% adjust center range of velocities
if counter == 1 || counter == 2
    % positive velocities
    centerRangeV = linspace(50,950,gridV);
else
    % negative velocities
    centerRangeV = linspace(-950, -50, gridV);
end

% if Cartesian coordiate,
if VXY,
    centerRangeA = linspace(400,700,gridA);
    centerRangeV = linspace(-400,400,gridV);
end

% 201 possible centers
for a = centerRangeA,
    
    % 201 possible centers
    for v = centerRangeV,
        allTol = zeros(size(AVD,1),1);
        
        % go through each trial
        for i = 1: size(AVD,1)
            
            ang = AVD(i,1) - center_a + a; % shifted angle
            vel = AVD(i,2) - center_v + v; % shifted velocity
            
            % looking up distance rather than computing it
            if ~VXY
                %%% NOTE! ADJUST scale according to your solution manifold
                % angle_index range: 1 to grain
                % ang: actual value for the x-axis of the solution manifold
                
                angle_index = round(ang * 10 + 1);
                
                % velocity_index range: 1 to grain
                % vel: actual value for the y-axis of the solution manifold
                
                %%% multiplier is (grain-1)/range of velocity, then add values needed to equal 1
                %%% range of velocity based on that given in generateSolM,
                %%% and vel = the bottom of the range given in generateSolM
                
                %%% adjust velocity index for positive and negative velocities
                if counter == 1 || counter == 2
                    % positive velocities
                    velocity_index = round(vel * 1800/1000 + 1);
                else
                    % negative velocities
                    velocity_index = round(vel * 1800/1000 + 1801);
                end
                
                if angle_index > 0 && velocity_index > 0 && angle_index < 1802 && velocity_index < 1802
                    Tol = D(angle_index, velocity_index); % read distance from dense matrix
                else
                    Tol = 1;
                end
                
            else
                angle_index = round( ang * 3 + 1);
                velocity_index = round( vel * 1.5 + 1201);
                
                if angle_index > 0 && velocity_index > 0 && angle_index < 2402 && velocity_index < 2402
                    Tol = D(angle_index, velocity_index); % read distance from dense matrix
                else
                    Tol = 1;
                end
                
            end
            % allTol: performance for whole block at this location
            allTol(i) = Tol;
        end
        
        T = zeros(size(AVD,1),1);
        posts = 0;
        v_n = 0;
        for j = 1:size(AVD,1)
            if allTol(j) == 1
                posts = posts + 1;
            else
                % add value only when allTol is not 1
                v_n = v_n+1;
                T(v_n) = allTol(j);
            end
        end
        
        % if all trials are posthit
        if v_n==0,
            T(1:end) = 1;
            
        elseif v_n<size(AVD,1),
            % m is maximum among the the non-posthit trials
            m = max(T(1:v_n));
            % replace posthit(=1) with m
            % T(v_n+1:end) = m;
            %%% depending on your algorithm, it could be either 1 or m.
            T(v_n+1:end) = 1; %if postHit = 1, use this line!
            
        end
        
        if mean(T) < idealMean % find location of best performance
            idealMean = mean(T);
            ideal_a = a;
            ideal_v = v;
            ideal_d = T;
        end
    end % loop through possible velocities
end % loop through possible angles

% generate the ideal AVD
idealAVD(:,1) = AVD(:,1)+ideal_a-center_a;
idealAVD(:,2) = AVD(:,2)+ideal_v-center_v;
idealAVD(:,3) = ideal_d;
% Tcost = (actual error) - (ideal error)
Tcost = mean(AVD(:,3)) - mean(ideal_d);
