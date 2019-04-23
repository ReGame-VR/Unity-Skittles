function simulation (subject, hand, visit, group, session)

%if you have to stop function early (using Ctrl+C after selecting console), type
%%'close all' (no quotes) in console to close figure, then
%%'close all hidden' (no quotes) in console to close message boxes

%% practice_instructions('name', 1, 'H', 1)
%%%^^^copy/paste this line into console, replacing 'name' and 'H', to run during instructions

disp('RESEARCHER: Did you check that the number of trials for this block is correct?');
disp('If yes, press any key to continue.');
disp('If no, quit with Ctrl+C and fix the trial number before running again.');
pause;

if visit == 1 && session == 1
    practice(subject,session,hand,1);
end

switch group
    
    case 1 %non-exploration group
        
        %if visit == 1
        
        skittlescore(subject, visit, session, hand, 1); %initial learning
        % else
        %     skittlescore(subject, visit, session, hand, 1); %retention
        
        %skittles_transfer(subject, visit, session, hand, 1);
        %%transfer - run after all retention blocks complete
        %end
        
    case 2 %exploration group
        
        % if visit == 1
        skittles_obstacles_for_exploration(subject, visit, session, hand, 1); %initial learning
        %else
        %    skittles_obstacles_for_exploration(subject, visit, session, hand, 1); %retention
        % end
        
    case 3 %transfer - for both exploration and non-exploration conditions, after visit 2 retention
        
        skittles_transfer(subject, visit, session, hand, 1);
        
end %end switch

end %end function