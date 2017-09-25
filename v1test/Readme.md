# 配置文件xcore.config
```
- TestUrl=http://192.168.2.3/ajax/shake?a=8&m=18&shake=1
- TaskCount=200
- BetweenTime=1
- ActiveInterval=1
- CacheResult=false
- WLN_REDIS_HOST=127.0.0.1
- WLN_REDIS_PORT=6379
- WLN_REDIS_PASS=
```
- TestUrl：测试的入口地址
- TaskCount：生成子线程的总数量（模拟用户数）
- BetweenTime：生成单个子线程的间隔时间
- ActiveInterval：线程活动间隔时间，单位为秒
- CacheResult：是否需要缓存测试结果

### 同时使用多台主机进行测试，配置Redis服务即可。