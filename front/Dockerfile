FROM node as dev
EXPOSE 80
WORKDIR /app

COPY ["front/package.json", "/"]
COPY ["front/package-lock.json", "/"]

RUN npm install

COPY ["front/", "/"]

CMD ["npm", "start"]