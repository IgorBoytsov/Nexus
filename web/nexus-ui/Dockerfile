FROM node:22-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci --frozen-lockfile

COPY . .

RUN npm run build -- --configuration production

FROM nginx:alpine AS prod

COPY --from=build /app/dist/nexus-ui /usr/share/nginx/html

EXPOSE 80