#ifndef __LIBJOY_H
#define __LIBJOY_H
void initJoystick();
void closeJoystick();
void moveJoystick(int xaxis, int yaxis, int zrotate, int button1);
#endif