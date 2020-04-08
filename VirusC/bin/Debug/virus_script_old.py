import sys
import numpy as np
import pandas as pd
import math
from datetime import datetime,timedelta
from sklearn.metrics import mean_squared_error
from scipy.optimize import curve_fit
from scipy.optimize import fsolve
import urllib.request


def logistic_model(x,a,b,c):
    return c/(1+np.exp(-(x-b)/a))

def exponential_model(x,a,b,c):
    return a*np.exp(b*(x-c))

def get_line_list(li):
    if li.startswith(b'"'):
        li1=li.split(b'",')
        li2=li1[1].strip().decode('utf-8').split(",")
        li2.insert(0,li1[0].strip(b'"'))
        return li2
    elif b'"' in li:
        li1=li.split(b'",')
        li2=li1[1].strip().decode('utf-8').split(",")
        li2.insert(0,li1[0].strip(b',"'))
        li2.insert(0,"")
        return li2
    else:
        return li.strip().decode('utf-8').split(",")

def getDataFromSource(url, country_in):
    f1 = urllib.request.urlopen(url).readlines()
    counter=0
    data=[]
    if country_in=='World':
        for li in f1[1:]:
            lst = get_line_list(li)
            if counter==0:
                pass
            elif counter==1:
                data_in = [int(i) if i!='' else 0 for i in lst[4:]]
                data=data_in
            else:
                data_in = [int(i) if i!='' else 0 for i in lst[4:]]
                data = map(lambda x,y:int(x)+int(y),data,data_in)
            counter+=1
    else:
        for li in f1[1:]:
            lst = get_line_list(li)
            if country_in in str(lst[0]) or country_in in str(lst[1]) or country_in in str(lst[2]):
                data_in = [int(i) if i!='' else 0 for i in lst[4:]] 
                if counter==0:
                    data=data_in
                    counter+=1
                else:
                    data = map(lambda x,y:int(x)+int(y),data,data_in)
                    counter+=1
    return data


print("Coronavirus COVID-19 distribution (ver. 1.1 - Cooknas 2020)")
print("Running in Python ver.: {}".format(sys.version))
#### getting the data from 
url_confirmed = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Confirmed.csv"
url_deaths = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Deaths.csv"
url_recovered = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Recovered.csv"
f1 = urllib.request.urlopen(url_confirmed).readlines()
p1=int(4)
p3=int(1000)
meth = 'dogbox'
#get input arguments
input=sys.argv
print(input)
country=input[1].strip("'")
if len(input)>2:
    p1=int(input[2])
if len(input)>3:
    p3=int(input[3])
if len(input)>4:
	meth=input[4]
lst=[]
dates1=f1[0].strip().decode('utf-8').split(",")[4:]
for i in range(0,len(dates1)):
    ll=dates1[i].strip().split('/')
    dates1[i]=ll[0]+"/"+ll[1]+"/20"+ll[2]
counter=0
data=[]
if country=='World':
    for li in f1[1:]:
        lst = get_line_list(li)
        if counter==0:
            pass
        elif counter==1:
            data_in = [int(i) if i!='' else 0 for i in lst[4:]]
            data=data_in
        else:
            data_in = [int(i) if i!='' else 0 for i in lst[4:]]
            data = map(lambda x,y:int(x)+int(y),data,data_in)
        counter+=1
    country="World"
else:
    for li in f1[1:]:
        lst = get_line_list(li)
        if country in str(lst[0]) or country in str(lst[1]) or country in str(lst[2]):
            data_in = [int(i) if i!='' else 0 for i in lst[4:]] 
            if counter==0:
                data=data_in
                counter+=1
            else:
                data = map(lambda x,y:int(x)+int(y),data,data_in)
                counter+=1

#print("args: country={} p1={} p3={}".format(country, p1,p3))
FMT = '%m/%d/%Y'
dates2=list(dates1)
dates =map(lambda x : (datetime.strptime(x, FMT) - datetime.strptime("01/01/2020", FMT)).days, dates1)
x = list(dates)
y = list(data)
y_d = list(getDataFromSource(url_deaths,country))
y_r = list(getDataFromSource(url_recovered,country))

for p2 in range(1,200):
    bfunc=False
    berr=False
    try:
        fit = curve_fit(logistic_model,x,y,p0=[p1,p2,p3],method=meth)
        bfunc=True
    except:
        #print("Error in fit_curve with p2={}".format(p2))
        bfunc=False
    try:
        errors = [np.sqrt(fit[1][i][i]) for i in [0,1,2]]
        berr=True
    except:
        #print("Error in calculating errors with p2={}".format(p2))
        berr=False
    if bfunc and berr and fit[0][0]>0 and fit[0][1]>0 and fit[0][2]>0:
        break
# calc also exponential
bexpfunc=False
try:
    exp_fit = curve_fit(exponential_model,x,y,p0=[1,1,1])
    bexpfunc=True
except:
    #print("Error in exponential solving")
    bexpfunc=False
# now print results
print("Results")
if bfunc and berr:
    sol = int(fsolve(lambda x : logistic_model(x,fit[0][0],fit[0][1],fit[0][2]) - int(fit[0][2]),fit[0][1]))
    print("{} {} {} {} {}".format(fit[0][0],fit[0][1],fit[0][2], sol, p2))
    print("{} {} {}".format(errors[0],errors[1],errors[2]))
    print("X")
    print("{}".format(x))
    print("Y")
    print("{}".format(y))
    print("Y_d")
    print("{}".format(y_d))
    print("Y_r")
    print("{}".format(y_r))
    if bexpfunc:
        print("Exponential")
        print("{} {} {}".format(exp_fit[0][0],exp_fit[0][1],exp_fit[0][2]))
    else:
        print("No Exponential solution")
else:
    print("No solution found so far")
    