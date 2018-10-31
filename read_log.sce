clear
clc
xdel(winsid())

//txt = mgetl("C:\prive\chauffage\log_wpf\chauffage\bin\Debug\log_30_10.csv")
txt = mgetl("C:\prive\chauffage\log_wpf\chauffage\bin\Debug\log_31_10.csv")
//csv = csvRead("C:\prive\chauffage\log_wpf\chauffage\bin\Debug\log_30_10.csv")
disp(size(txt))

function [tick] = GetTime(timeAsString)
    time = strsplit( timeAsString , '_') 
    tick = datenum( [2018, strtod( time([2,1,3,4,5])' ) ] )    
endfunction

header = strsplit(txt(1) ,',')'

start_tick = GetTime( strsplit(txt(2) ,',')(1) )

ind = 1
for i = 2:size(txt)(1)
    split = strsplit(txt(i) ,',')
        tmp = ( GetTime( split(1) ) - start_tick)*24
        t(ind) = tmp
        val(ind,:) = split'
        val(ind,1) = string(tmp)      
    ind = ind+1
end

val( find(val=="C") ) = "0"
val( find(val=="O") ) = "1"
val = strtod(val)
//disp(t)

//disp(val(1,:))

disp( header )

// Colors
// 1 : noir
// 2 : bleu
// 3 : vert
// 4 : bleu claire
// 5 : rouge
// 6 : violet
// 7 : jaune
// 8 : blanc
// 9 : bleu

//temp_5 = 17

//temp_1 = 5

room_data_size = 7
temp_start = 34
temp_end_0 = 35
open_0 = 36
targ_0 = 32
meas_0 = 33

room = 1
temp_end = temp_end_0 + room_data_size*room
open = open_0 + room_data_size*room
targ = targ_0 + room_data_size*room
meas = meas_0 + room_data_size*room

scf()
title("Room " + string(room))
subplot(2,1,1)
plot(t,val(:,temp_start)); gce().children.foreground = 2; gce().children.thickness = 2
plot(t,val(:,temp_end)); gce().children.foreground = 6; gce().children.thickness = 2
plot(t,val(:,targ)); gce().children.foreground = 3; gce().children.thickness = 2
plot(t,val(:,meas)); gce().children.foreground = 5; gce().children.thickness = 2
ax = gca()
ax.data_bounds=[0,17,0;max(t),22,0]
//ax.y_label.text = "Dout# (LSB)"; 
ax.x_label.text = "Time [h]"; 
ax.font_size = 3
ax.x_label.font_size = 4
ax.y_label.font_size = 4
legend( header([temp_start,temp_end,targ,meas]) )
xgrid

subplot(2,1,2)
plot(t,val(:,open)); gce().children.foreground = 2; gce().children.thickness = 2
ax = gca()
ax.data_bounds=[0,-2,0;max(t),2,0]
//ax.y_label.text = "Dout# (LSB)"; 
//ax.x_label.text = "Time (10ms)"; 
ax.font_size = 3
ax.x_label.font_size = 4
ax.y_label.font_size = 4
legend( header([open]) )
xgrid
