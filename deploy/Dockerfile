FROM mcr.microsoft.com/dotnet/core/aspnet:3.0

RUN apt-get update
RUN apt-get install libc6-dev -y
RUN apt-get install libgdiplus -y

WORKDIR /app

# For NPOI
RUN apt-get update && apt-get install -y --allow-unauthenticated libgdiplus libc6-dev libx11-dev xvfb && ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
ENV DISPLAY :10
COPY xvfb.init /etc/init.d/xvfb
# Removes any windows line endings because linux can`t start the service in this case
RUN sed -i -e 's/\r//g' /etc/init.d/xvfb
RUN chmod +x /etc/init.d/xvfb
RUN update-rc.d xvfb defaults

COPY ITLab-Back .

ENV ASPNETCORE_URLS=http://*:5503
CMD service xvfb start ; dotnet BackEnd.dll
