---
Title: Deploying Then Securing the OWASP Juice Shop, Part Two of ? 
Lead: Manual deployment to AWS
date: 2023-08-10
draft: false
Tags:

- Amazon EC2
- Amazon Lightsail
- AWS
- AWS Elastic Beanstalk
- Cloud
- Deployment
- OWASP Juice Shop
- OWASP

Series:
  - Deploying Then Securing the OWASP Juice Shop
---
# Deploying the Juice Shop to AWS, the manual way

This post covers various attempts to deploy the OWASP Juice Shop (OJS) application on AWS.  Multiple approaches are trialled, with the comment element between them being that these are all fairly manual 'point-and-click' style methods.  Good for getting oneself up and running the first time, while getting to grips with AWS.  Not so good for reliable, reproducible deployments, however.  For the purposes of the remainder of this series of blog posts, I will be using OJS [v15.0.0](https://github.com/juice-shop/juice-shop/releases/tag/v15.0.0), which was the latest official release at the time I started.

_Disclaimer:  I am_ not _an expert on AWS.  I am experimenting here to practice and get more experience with using AWS.  Everything I describe below may well be_ bad _practices, never mind simply not good practices.  You should use caution and good judgement when attempting to copy anything I have done here._

## Possible Juice Shop Deployment Methods

The [OWASP](https://owasp.org/www-project-juice-shop/) [Juice Shop](https://github.com/juice-shop/juice-shop) project itself suggests a way to deploy the application.  Basically, stand up an [EC2](https://aws.amazon.com/ec2/) instance, pull in the OJS' Docker image, and fire it up.  The fact that it has a Docker image also means (so far as I can tell at this point) that there are _at least_ three more possible ways to deploy the application into AWS, using [Lightsail](https://aws.amazon.com/lightsail/), [Elastic Beanstalk](https://aws.amazon.com/elasticbeanstalk/) or [App Runner](https://aws.amazon.com/apprunner/).[^whatsthedifference],[^actuallyfour]  There's also a [Vagrantfile](https://developer.hashicorp.com/vagrant/docs/vagrantfile), meaning that standing up a full self-contained VM for it should be reasonably simple, too—although, that's not _really_ Vagrant's intended use case.[^novagrant]  At the very least, the Vagrantfile should give a pretty good guide as to what you should do yourself to configure a fresh [Amazon Machine Instance](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/AMIs.html) to make it work with the OJS.  

[^whatsthedifference]:  To be completely honest, as at the time of writing I can't actually tell what the important differences are between Lightsail using containers, Elastic Beanstalk and App Runner.  Except that the first two qualify for a free tier for the first 12 months after account creation (transitively via Beanstalk, apparently), while the latter seemingly has no free tier.  I haven't worked with any of them in depth yet, though.
[^actuallyfour]:  Actually, at least four.  I think.  There's also the [Elastic Container Service](https://docs.aws.amazon.com/ecs/index.html).  As with the other three, though, I'm not too sure at this point what the differences are.
[^novagrant]:  In fact, I couldn't find any reference to Vagrant in AWS' official documentation, and almost nothing mentioning Vagrant in the AWS Marketplace.  Mitchell Hashimoto himself seemingly used to provide an AWS plugin to Vagrant, but that has apparently been deprecated now, with no indication of any sort of replacement or alternative.

The OJS itself, independent of deployment/development environment helpers, is a Node.js application.[^nodeversion]  If one really wants to perform the whole process oneself (rather than bundling everything up into a container or VM), it is one of the easiest languages/ecosystems to do-it-yourself with.  Perhaps following the Vagrantfile, as mentioned above.  One mildly interesting aspect of OJS is that it does _not_ require a separate database.  I haven't dug into the codebase much yet (nor do I have all that much experience with non-trivial Node.js), but it seems, best as I can tell, that it incorporates SQLite into itself.  E.g. OJS v15.0.0 lists `sqlite3`, with versions equal to or above 5.0.8, as part of its dependencies in its package.json file.  This does make deploying it much easier, but isn't wholly realistic to most real-life web applications, which usually use one or more external DBMSes.[^dockercomposedbms]

[^nodeversion]:  I targeted Node.js 18, since it was listed as the latest version of Node.js that was still under support by both Node.js and OJS.  I actually suspect the table listing the supported versions that I saw was probably out-of-date, but I figured I should just stick with the documentation.  Plus, the Dockerfile in the OJS release I'm using still specifies the use of Node.js 18 images.
[^dockercomposedbms]:  An excellent example of a well-put-together Docker Compose system for this sort of thing can be found in a [Containerised MISP distribution](https://github.com/NUKIB/misp) created by what I understand to be the Czech equivalent of the US' CISA.  It includes not only a running MISP, but the relevant backing MySQL and Redis containers, too.  I used it for some local testing against a live MISP instance while at Cosive, and it worked really well. 

## First Look at the OWASP Juice Shop

Before trying to deploy OJS online, it seems like a good idea to try to run it locally and get some impression of what a running instance should look like.  To that end, I decided to download a copy of the official Docker image, and run that according to the provided instructions.  A simple pull of the image and a `docker run --rm -p 3000:3000 bkimminich/juice-shop:v15.0.0` later, I have OJS running locally.  Et voilà 🎉

{{< figure src="OWASP_Juice_Shop_start.png" title="The OWASP Juice Shop front page on start up." alt="An image displaying the front page with of the OWASP Juice Shop, overlaid with a pop-up box explaining the purpose of the application." >}}

For now, I'm _not_ going to attempt to poke any holes into it.  That will come later once I have done a few different deployment methods.  I did find the scoreboard, though, which I understand is considered the very first challenge of the whole thing.  That particular challenge was simple enough that even I could do it without help (and a penetration tester I am _not_).

## EC2 Instance

For my first ~~trick~~ deployment, I figure I will just follow the instructions provided by the [OJS project itself](https://github.com/juice-shop/juice-shop/tree/master#amazon-ec2-instance) to deploy onto an EC2 instance.  They seem to be pretty simple & straightforward.  Though, since I'm not an AWS expert, the number of configuration options for a new EC2 instance seem somewhat overwhelming and perplexing.  For my purposes, I'm going to assume that the defaults will do if the OJS instructions haven't mentioned changing them.  This isn't completely a reasonable assumption, since things change in AWS all the time, but let's hope it is OK for this.

### Not Quite There

The instructions were pretty good (as good as you could reasonably expect for a free, third-party project).  Unfortunately, I think I fell down on the part about allowing inbound HTTP traffic in the security groups.

#### Second Time's the Charm

So I went back and re-did the deployment, this time ensuring that I assigned a security group with a rule allowing all inbound IPv4 HTTP traffic.  After a couple of minutes or so, I could indeed connect to the instance and start poking around in it.  I confirmed that I could still get to the scoreboard, and that I hadn't somehow had the same user that I registered in the local instance come across to the online instance (I have zero clue how that could happen beyond sorcery, but I figured I should try it just in case).

So that first go can be declared a success! 🥳  I terminated the instance moments later, of course, since I had done what I needed to do with it.  Or rather, I had copy-pasted what I needed from the OJS instructions into the relevant box on the instance launcher page.  Specifically, this:

```shell
#!/bin/bash
yum update -y
yum install -y docker
service docker start
docker pull bkimminich/juice-shop
docker run -d -p 80:3000 bkimminich/juice-shop
```

{{< figure src="OJS_EC2_Instance_Creation_User_Script.png" title="Setting the commands to run when the EC2 instance is launched." alt="A screenshot showing some of the settings for a new AWS EC2 instance.  In particular, this screenshot shows the commands to be run when the instance launches." >}}

Either way, it was running in the cloud.

### Using a Load Balancer

The above deployment worked, in that I could indeed reach an instance of OJS running in the cloud.  I am not at all happy with the fact that it is using HTTP, rather than HTTPS, though.  I could try to add in support for HTTPS to OJS (or, at least, figure out how to deploy it behind Nginx or something).  If I were planning to deploy it long-term onto my own server, that would probably be necessary.  There are two main problems with that, however:

- It might be a lot of extra complexity orthogonal to the intended purpose of OJS.  I can't be bothered with all that right now.
- I'm not even familiar with the codebase for OJS, so trying to retrofit HTTPS support for it would probably take me some considerable effort—and these blog posts already take me much longer than I would prefer.

Since I'm experimenting in this post with deploying to AWS specifically, I might as well reach for an AWS-specific solution.  That, generally, means slapping a load balancer in front of OJS.  This load balancer both can act as a literal load balancer, divvying up requests between instances of a running application.  It can potentially also be used for multiple other purposes beyond that.  For one thing, you can create a private subnetwork in AWS (Amazon VPC), and make the load balancer the only public facing part of it.  You can also use it as your SSL termination proxy, and (as I understand it) the place where you deploy a web application firewall.  In fact, the first two of those additional purposes pretty closely match what the Lightsail container service (discussed below) provides automatically as part of its value proposition.  Essentially, putting a load balancer in front of your application lets you abstract over it and expose a tidy and rational interface to the outside, while you do whatever wackiness is needed behind the scenes.  An additional bonus is that, if I set it up correctly, I don't have to make a single change to OJS to get it all working properly, but only accessible via HTTPS.

Of course, the updated picture isn't completely rosy.  Using a load balancer inherently means extra complexity, both in terms of AWS setup but also for the application itself in certain circumstances (mostly relating to whether the application itself needs to know where a request is coming from, I believe).  You also only have so much control over the configuration and behaviour of the load balancer.  For the majority of use cases, I'm pretty sure it won't make a blind bit of difference, but sometimes this might rear its ugly head.  For the current purpose of just doing a deployment proof-of-concept, I doubt that any of the extra drawbacks beyond the complexity will come into it.

In AWS, the usual way to incorporate loading balancing is to use [Elastic Load Balancing](https://docs.aws.amazon.com/elasticloadbalancing/index.html).  More specifically, my understanding is that you should use an [Application Load Balancer](https://docs.aws.amazon.com/elasticloadbalancing/latest/application/) by default (unless you know you need a different one).  Of course, the actual process to get everything set up is a little more complex than simply logging into AWS and telling it to do load balancing.

There are _at least_ three components required to get this all up and running.  Namely

- The EC2 instance running the OJS container, as before
- A second EC2 instance, since it seems that the Application Load Balancer requires at least two running instances in separate Availability Zones.  This is likely extremely sensible for any application that is intended to be long-running, but not so relevant here.
- The Application Load Balancer to sit between the EC2 instance and the wider internet
- A Virtual Private Cloud that the EC2 instance sits inside, inaccessible to the outside world except by going through the load balancer, which is placed on the edge of the VPC.[^alreadyvpc]

[^alreadyvpc]:  In fact, the original EC2 deployment also used a VPC.  Just, it used the default VPC that AWS creates and assigns automatically, meaning that there was zero configuration needed, and it disappeared into the background. 

#### Virtual Private Cloud Setup

Every AWS account [automatically gets](https://docs.aws.amazon.com/vpc/latest/userguide/default-vpc.html) default VPCs in each region, with default subnets in each Availability Zone.  Since it might make something else more difficult to do in the future, I don't want to start fiddling with the default VPC for the region I use, however.  Thus, I will create a special-purpose VPC for this test deployment.  I will start with following the [example VPC for a test environment](https://docs.aws.amazon.com/vpc/latest/userguide/vpc-example-dev-test.html), and then tweak the configuration from there as needed.  This basically just involves creating a single VPC with a single public subnet, into which an EC2 instance can be deployed.  It also largely involves simply accepting the defaults in the VPC creation web UI.

{{< figure src="network-arrangement-diagram.png" title="AWS' conception of how the pieces of my new VPC fit together." alt="A screenshot of a diagram showing boxes connected by lines, where those boxes represent, from left to right, the new custom VPC, the VPC's subnet, the associated route table, and the associated network connection." >}}

##### Redeploying to an EC2 instance

As a first step to check that this new VPC does indeed work, I'll redo the steps for deploying the OJS into an EC2 instance, but using this VPC instead of the default one.  Weirdly, despite my having selected the dedicated OJS VPC, the interface still selected the default VPC's subnet.  I only realised this had happened when I tried to start the instance and the webform complained.  Furthermore, I clearly missed an important setting, because on starting up the instance I realised that there was no public IP address assigned.  This is, of course, eventually the intended behaviour (and is _definitely_ a more secure default than the opposite), but it caught me by surprise nevertheless.  I couldn't figure out how to reconfigure my running instance to get it to do automatic IPv4 assignment, so I ended up terminating it and starting again.  I'm certain there must be a proper way to do this, but it wasn't obvious to me.

{{< figure src="Auto-assign_public_ip.png" title="Probably the bit I missed first time around." alt="A screenshot highlighting the 'auto-assign public IP address' option when launching a new EC2 instance." >}}

At first, I puzzled over why I seemingly could not connect to this instance.  I had waited several minutes for the boot-up process to finish.  The monitoring showed that, most likely, the OJS Docker image had indeed been downloaded shortly after the instance started.  Not even a reboot fixed things.  I figured that maybe using the DNS name for it might not work immediately, since it could well take some time for DNS to propagate, but I would have thought that connecting to the IP address directly should be fine.  Eventually, I noticed that the inbound traffic rule on the associated security group permitted all protocols on all ports, but that it was restricted to other sources within the same security group.  Thus, I added another inbound rule permitting HTTP traffic from anywhere (to be removed again once I start using the load balancer, of course).

In the end, I had to terminate the instance and start again before I could _finally_ connect to it via HTTP again.  I'm not sure what I could have done better, but almost certainly there was something.  The couple of issues described above are definitely traps for the unaware.  There's also definitely more complexity available to those who want to dig into it, but I'm not a networking guru, so I'm happy to go with defaults which _seem_ to work.  For example, you can fiddle with internet gateways for your VPC, but the default seemed to what I needed do I never dug further into it.

{{< figure src="Create_load_balancer_option.png" title="Oh look.  How handy (in future)." alt="A screenshot highlighting an option to create a load balancer, presented immediately after launching a new EC2 instance." >}}

#### Creating a Load Balancer

Before I even start on creating the new load balancer, I apparently need to ensure that my VPC has _two_ Availability Zones set up (according to the [introductory documentation](https://docs.aws.amazon.com/elasticloadbalancing/latest/application/application-load-balancer-getting-started.html#prerequisites)).  I originally set the custom OJS VPC up with only one, following the documentation for creating a VPC for a test environment.  Each subnet for a VPC is assigned to a specific AZ, so I would need two subnets under the VPC to make it all work, rather than the one that I have now.  Given that I don't plan to keep any of these resources around for long, I think I'll just use the VPC creation wizard to create a new VPC which has the two subnets for two AZs out of the box.  For this one, I set the number of _public_ subnets to zero, since I don't plan to access either of the EC2 instances directly.

With that now in place, the next step is apparently to get two EC2 instances up and running.  As before, I'm just going to imitate the initial EC2 deployment into the VPC, but I very deliberately will not choose to have IPv4 addresses assigned.  These instances should _not_ be accessible from the outside world.  Best as I can tell, if I just stick with the default security group, which allows access via all protocols and ports but only from within the VPC, then when I go to use the load balancer everything should still "just work".

##### Create a Target Group

This bit basically bundles the EC2 instances up into a package that the load balancer can be pointed at, and seems to be a necessary step for setting up a load balancer.  There's not really much more to it than to choose the right VPC and then include the instances under it.

##### Create and Configure an Application Load Balancer

With the target group established (I'm not 100% clear on why they need to make them separate from the load balancers, but I'm sure there's a darn good reason), I can finally get to creating the load balancer itself.  One catch I discovered quickly into this process is that the VPC I set up for this can't be selected for the load balancer's network mapping.  As per the instructions, "Only VPCs with an internet gateway are enabled for selection."  It seemingly turns out that I still need an internet gateway even if I don't intend for the EC2 instances in the VPC to be accessible, which is different to my previous understanding of all this.  Fortunately, it seems that creating and attaching an internet gateway is a relatively trivial process.  IGs don't need configuration, and attaching them to a VPC is pretty much painless.

Turns out (rather unsurprisingly, really) that as a flow-on effect from not creating the VPC with an internet gateway to begin with, the VPC's subnets are not appropriately configured to use the IG.  Thus, I need to update the subnets' route tables.  It wasn't immediately obvious how to configure the route table's appropriately, but fortunately I still had the previous working VPC/subnet/route table still hanging around.  From looking at that instance's subnet route table configuration, it seems that I should (probably) configure 0.0.0.0/0 to associate to the freshly-created IG from just above.  Beyond that, however, the rest of the load balancer configuration pretty much just follows the defaults, for now.  Importantly, it seems that the default load balancer setting just creates an HTTP endpoint, and _not_ and HTTPS one.

Of course, there was one rather glaring issue with the two EC2 instances that I had started earlier...  Without an associated internet gateway, they had no access to the internet, and thus were unable to download the relevant Docker image, so there wasn't anything to note running inside them for the load balancer to forward requests to.  Given that the all-important user data script only seems to run on the first start-up, I had to go create new instances derived from the old ones, to try to make things work again.  This also meant that I had to go update the registered targets in the target group I created earlier, to encompass the new instances and exclude the old ones.  Sadly, this doesn't seem to have covered everything.  The target group continues to show both targets as unhealthy, despite having switched to the new instances and those instances showing as healthy in their own checks.  Moreover, those instances both appear not to have downloaded the Docker image (going by the inbound traffic volume metric).

{{< figure src="Target_Group_Needs_Configuring.png" title="Whoops, the target group needs updating." alt="A screenshot showing a warning message from AWS indicating that a subnet for the target group does not have an assigned internet gateway, and thus needs to be reconfigured appropriately before it can be used." >}}

It would appear that I still haven't quite got all the settings correct.  My best _guess_ is that I didn't quite configure the inbound rules on the security group those instances use so that the target group/load balancer could actually talk to them.  To test that out, I tried adding in a new inbound rule to the security group, one which permits all traffic in.  That doesn't seem to help, however.  I noticed on the target group's monitoring that the number of requests recorded as received went up from zero, and instead of my browser just eventually timing out while trying to connect to the load balancer's DNS address, it now finishes quickly with a 502 "Bad Gateway" error.  Clearly, even though the EC2 instances are supposed to be private and inaccessible by the outside world, the associated security group still needs a rule to permit traffic in from the wider world.  No change to the health (or not) status of the hosts within the target group, though.  They're still marked as unhealthy.

At this point, I'm rather stumped as to what the issue is, and I'm getting bored with this whole rigmarole.  Since there's no actual requirement to get this working, I have decided at this point to give up on it _for the time being_.  I'll have to remember to come back to it all later on, but for now I just clean up all the non-default resources that I created.  If anybody out there knows what I did wrong, and more importantly what I should do differently, please let me know.  

## Lightsail

Next, I decided I would try to do a deployment via AWS' Lightsail service.  To the best of my understanding, it pretty much lets you do the same as what was involved in setting up the EC2 instance described above, but with many of the knobs simply set to reasonable defaults that should serve most purposes.  In particular, I _think_ it one uses Lightsail, it takes care of handling SSL/TLS certificate provision for you (I presume via [Let's Encrypt](https://letsencrypt.org/)), so I believe I should be able to talk to an instance of the OJS via HTTP**S**, rather than plain HTTP.  Which is very much to be favoured, given that security is a primary concern of this whole project.  Also, since we have a pre-made container image, we might as well re-use that and focus on using Lightsail's support for containers, rather than try fiddling to configure a full instance.

According to the [overview of Lightsail's container service](https://lightsail.aws.amazon.com/ls/docs/en_us/articles/amazon-lightsail-container-services), not only does it handle setting up HTTPS for you, but it also handles load balancing across running instances of the container.  Essentially, at the most basic, you just configure it to say how powerful you want the compute nodes to be, and how many running instances you want operating behind the load balancer, and it handles the rest of the process.  It's not cheap, however (emphasis added).

> The monthly price of your container service is calculated by multiplying the price of its power with the number of its compute nodes (the scale of your service). For example, a service with a medium power, which has a price of $40 USD, and a scale of 3 compute nodes, will cost $120 USD per month. **You are charged for your container service whether it's enabled or disabled, and whether it has a deployment or not.** You must delete your container service to stop being charged for it.

It's a good thing I'm not planning to leave an instance of this lying around!  AWS provides some Lightsail services on the 12-month free tier, but I don't fancy suddenly receiving a USD$120 charge out of the blue in a year's time...  In the context of the OJS, where – if I even wanted it deployed the cloud – I could get away with a single low-power instance, I think I'd choose the EC2 approach described above.  I can see the utility to Lightsail for certain (probably not-very-technical) users and use cases though.  Also, if I'm understanding the [pricing page](https://aws.amazon.com/lightsail/pricing/) correctly, it seemingly turns out that the Lightsail container service isn't normally available under the free tier.  Happily, as at the time of writing they do give you three months free on their Micro power level, with a scale of 1 node.  Given that I don't plan to keep this service around for longer than (at most) half an hour, that should do for my ends.

### Deployment

It turns out that the main 'getting started' docs for Lightsail's container service actually seem to be pretty good (unlike [some other](../onepointfive) getting started docs I could mention...).  Most notable is this section:

> Create a container service with a deployment if you plan to use a container image from a public registry. Otherwise, create your service without a deployment if you plan to use a container image that is on your local machine. You can push the container image from your local machine to your container service after your service is up and running. Then you can create a deployment using the pushed container image that is registered to your container service.

Given that I'm planning just to deploy the public OJS image already found [on Docker Hub](https://hub.docker.com/r/bkimminich/juice-shop), I'll set the deployment up _before_ creating the service.  I'm not 100% sure about what the correct configuration is, but I'll make my best guess based on the provided deployment instructions for the EC2 instance.  Which makes it seem like the whole thing is pretty self-contained, and all I probably _really_ have to do is ensure that requests are sent to port 3000.

{{< figure src="OJS_Lightsail_Container_service_deployment_1.png" title="My first best guess at the correct deployment configuration" alt="An image showing a configuration interface for setting up a Lightsail container service deployment." >}}

Even for something as relatively simple as the OJS, deployment takes a good couple of minutes or so.  Interestingly, attempts to navigate to the assigned semi-random URL return a 503 error with the message "Service Temporarily Unavailable" if you try to navigate to the page before the deployment has finished.  Happily, it appears that I got the deployment configuration correct!  Once the container service status rolls over to 'Running', I can access an instance of the Juice Shop online.  Taking a quick squiz at the CPU utilisation logs, it appears that the service faced an initial spike to around 40% of total available CPU around start up time (possibly when I first accessed the site(?)), but quickly dropped back to much lower—with the next highest peak (probably when I left a fake feedback comment) at a mere 15%.  Similarly, memory use spiked shortly after launch to 20.56% of the available amount, and then hit a steady state of around 17%-19%.  I'm unconvinced that a single Micro instance has the grunt to power an _actual_ e-commerce site, but it seems to do the trick for a single-user exemplar.  Of course, moments later I shut the whole thing down and deleted the service, since I don't want to be charged for it!

{{< figure src="OJS_Lightsail_Instance_CPU_util.png" title="CPU utilisation when running the OJS in a Lightsail container service" alt="A chart showing movements over time in the utilisation of the virtual CPU in the Lightsail container instance." >}}
{{< figure src="OJS_Lightsail_Instance_Mem_util.png" title="Memory utilisation when running the OJS in a Lightsail container service" alt="A chart showing movements over time in the utilisation of the allocated memory in the Lightsail container instance." >}}

### Lightsail Instance, Without the Container Service?

Given that there's (almost) no such thing as a free tier in the Lightsail Container Service, can I deploy the OJS to a regular Lightsail instance, and thus hopefully enjoy the usual free tier rate?  Perhaps it can be treated similarly to the EC2 instance, where basically the only thing done is to ensure that Docker is installed, pull the OWASP Juice Shop image and run it, then check the exposed endpoint a few minutes later.  Possibly if I just use the Amazon Linux AMI again (as done above), it'll all work.  Considering the performance metrics discussed above, I'll just go with the smallest of the free tier options, since it seems like that should cover it, and I'd prefer to minimise any surprise bills down the line.  Let's see how this goes...

The Lightsail instance itself gets up and running _much_ more quickly.  That probably reflects, however, the fact that there is a lot more work to be done on the initial startup compared to the container service deployment, which apparently handled absolutely everything for us.  Despite waiting a fair number of minutes, I'm unable to access the OJS via the IPv4 address that AWS assigned.  Eventually, I twig that half the problem is probably that the OJS listens on port 3000, and I'm trying to contact it on port 80 (i.e. the norm for HTTP).  Except, then I remember that it should just be running a Docker container, where port forwarding has been set up between ports 80 and 3000.  Just in case, I still try opening up port 3000 in the networking tab of the instance status page, and attempt to point the browser there.  No luck, however.

Furthermore, all attempts to SSH to the server fail, both by using AWS' own browser-based SSH, and trying to use the Windows 11 built-in SSH client.  Basically, in neither case does it seem like the server actually responds, although nothing times out either, so I'm not too sure what is really happening.  I can see on the CPU utilisation graph that it peaks at about 85% roughly 20 minutes into the deployment time, but still no luck connecting.  Attempts to navigate to the provided IP address all just a 'connection reset' error, and attempts to SSH in never resolve, and I have confirmed that port 22 is listed as open.  Most curiously, when I check the measure of incoming network traffic, there's about 30 MB listed on the first measurement after start up, but absolutely zero after that.  Given that I thought the Docker container was considerably larger than that, it looks to me like perhaps the instance never got to the point of downloading it.  There's no instance of status check failures, however, so it seems like probably _something_ in the deployment is still live and responding.  The exact same thing seems to continue after rebooting the instance.  I'm not sure what the problem was (though I assume I didn't configure everything correctly), but this experiment should probably be regarded as a failure.  To make matters worse, when I try to stop the instance AWS bills me $0.51.[^monthlybill]  The instance takes several minutes to stop, though, leading me to wonder if there was somehow and infinite loop or something going on inside.

[^monthlybill]:  Turned out that was actually the total charges for the previous month, and the timing was just an incredible coincidence.  The charge appears to suggest extremely strongly that one of the tutorials I did which claimed to be entirely on the free tier actually wasn't, though...

### Lightsail Instance, Without the Container?

The previous experiment more-or-less tried to replicate the EC2 deployment, but in a Lightsail instance.  Something about that, however, didn't work.  Perhaps AWS put something in to stop people from essentially replicating their Lightsail container service on the cheaper regular instances.  I don't know.  There are also instructions on the OJS GitHub page about building and running the whole thing from source or by downloading and unpacking a prepackaged release onto an environment with Node.js already installed.  It appears that Bitnami packages a Node.js AMI that can be used in Lightsail, so maybe trying these instructions using that will work better.

{{< figure src="OJS_Lightsail_Instance_Creation_Choose_Instance_Image.png" title="Bitnami's Node.js AMI for use in Lightsail" alt="A screenshot depicting the selection of Bitnami's Node.js Amazon Machine Image onto which to deploy an instance of the OWASP Juice Shop." >}}

#### From Source

This time around nobody has provided a ready-to-copy code block, but the OJS instructions are still fairly detailed.  It shouldn't be too difficult to turn those into a startup script.  Which ends up looking like this (at least for my first attempt):

```shell
git clone https://github.com/juice-shop/juice-shop.git --depth 1
cd juice-shop
npm install
npm start
```

This time around, there is no container port forwarding, so I probably will indeed need to access the site via port 3000.  Thus, I probably need to go expose port 3000 through the firewall again.  After doing so, I checked the metrics, and was surprised to see very little in the way of CPU utilisation and incoming network traffic.  It looks like, for some reason unclear to me, the startup script I supplied doesn't seem to have worked.  This time, however, when I try to SSH into the machine, it actually responds and I can poke around in the remote terminal.  There's no sign of the git repository in the home directory, so perhaps things fell at the first hurdle.  Let's see if I can do it all manually, at least.

Actually, after running `git clone`, I noticed that the total size of the repo appeared to be approximately the same as the amount of the incoming network traffic.  It turns out that the OJS repo was cloned under the top-level directory (i.e. `/`), but I suspect the other commands might well have been run from within the home directory.  In the future, I'll have to try to ensure that I clone things to the home directory specifically.  Now to wait while `npm install` does its thing and downloads approximately 1 million packages.  A few minutes later, it aborts with an out-of-memory error.  Presumably that was the _real_ reason why nothing happened.  Right, seems that if I want to build the application locally, I'll need more than 512 MB.  I'm not surprised, but I did kinda hope.  Ah well, I'll try to upgrade this instance's scale to the next level of the free tier.

This time around, I went with the largest of the free tier options (which has four times the RAM), but since I'm upgrading based on a snapshot of the previous deployment, I don't bother to set up the initial deployment script again.  Instead, I'll just SSH in again and continue the process manually.  Except, I then get an error telling me that I'm not allowed to create a new instance from the snapshot with that size.  So I go with the middle option, that only has double the RAM.  Hopefully, that'll still be enough.

This time around, the installation process doesn't fail with an out-of-memory error.  Instead, there are a bunch of deprecation warnings (which I assume are a deliberate part of making OJS an _intentionally_ vulnerable web application), and then a failing error which I don't quite understand.  I have included it below in case someone more familiar with Node.js than me can explain what it is about.

```shell
npm ERR! code 1
npm ERR! path /home/bitnami/juice-shop/node_modules/cypress
npm ERR! command failed
npm ERR! command sh -c node index.js --exec install
npm ERR! /home/bitnami/juice-shop/node_modules/isexe/index.js:25
npm ERR!         if (er
npm ERR!               
npm ERR! 
npm ERR! SyntaxError: Unexpected end of input
npm ERR!     at internalCompileFunction (node:internal/vm:73:18)
npm ERR!     at wrapSafe (node:internal/modules/cjs/loader:1178:20)
npm ERR!     at Module._compile (node:internal/modules/cjs/loader:1220:27)
npm ERR!     at Module._extensions..js (node:internal/modules/cjs/loader:1310:10)
npm ERR!     at Module.load (node:internal/modules/cjs/loader:1119:32)
npm ERR!     at Module._load (node:internal/modules/cjs/loader:960:12)
npm ERR!     at Module.require (node:internal/modules/cjs/loader:1143:19)
npm ERR!     at require (node:internal/modules/cjs/helpers:110:18)
npm ERR!     at Object.<anonymous> (/home/bitnami/juice-shop/node_modules/which/which.js:7:15)
npm ERR!     at Module._compile (node:internal/modules/cjs/loader:1256:14)
npm ERR! 
npm ERR! Node.js v18.16.1
```

It does occur to me that I forgot to set the cloned git repository to the relevant commit/tag, so I go back and ensure that I have checked out the `v15.0.0` tag and try `npm install` again.  I hit the exact same result in the end though.  To be honest, I can't be bothered trying to debug this any further right now, so I'll just move on to trying to install a pre-packaged release.

#### From A Release

The OJS team publish [pre-packaged releases](https://github.com/juice-shop/juice-shop/releases/tag/v15.0.0) of the project, as well as things like the Docker images.  By my estimation, given that the version of Node currently packaged into the Bitnami AMI is v18, I think I want the v15.0.0 node18 Linux .tgz.  Given that I already have a working Node.js Lightsail compute instance from the attempt to install from source, I'm just going to re-use that.  The instructions for using one of the releases amount to executing the following commands (yes, it turns out I probably could have just extracted to the home directory and skipped the first level of `juice-shop`):

```shell
wget https://github.com/juice-shop/juice-shop/releases/download/v15.0.0/juice-shop-15.0.0_node18_linux_x64.tgz
tar -xzvf juice-shop-15.0.0_node18_linux_x64.tgz -C ~/juice-shop
cd ~/juice-shop/juice-shop-15.0.0
npm start
```

Let's see if this does any better.  The output from the terminal after running the start command appears to match that seen when using the Docker container, so that seems promising.  I first try to access the page by browsing straight to the provided IPv4 address (unlike with the container service, using a regular Lightsail instance doesn't seem to allocate a URL automatically).  Of course, this first hits the problem that it's HTTP-only, and my browser is set to HTTPS-only mode.  Overriding that and continuing to navigate to the address lands me on a welcome page for the Bitnami Node.js image.  Trying to navigate to the same IP address on port 3000 doesn't seem to get me anywhere.  Then I realise that I still haven't opened port 3000 in the network settings for the instance.  After I correct that, I can indeed access the OJS in the cloud again.  After about 5 or so minutes, however, I suddenly can't get the server to respond on port 3000 to anything.  I'm not too sure what happened, but the application seemingly stopped running of its own accord.  Started working fine again as soon as I restarted it, though.

{{< figure src="OJS_Lightsail_Instance_Bitnami_NodeJS_frontpage.png" title="The Bitnami Node.js introductory front page, seen when accessing the assigned IP address via HTTP on port 80." alt="A screenshot of the welcome page for the Bitnami Node.js AMI." >}}

Anyway, now that I have confirmed that I can run the OJS in a regular Lightsail container (completely manually, at least), I'll delete it and move on to trying the next approach.

## AWS Elastic Beanstalk

It turns out that [Elastic Beanstalk](https://aws.amazon.com/elasticbeanstalk/)(EB) is kind of part-way between EC2 and Lightsail.  Essentially, it helps to orchestrate a bunch of the minutiae of doing a deployment, such as setting up a load balancer appropriately.  You don't actually create EB resources directly, but rather you use it to automate (partially) the setup of things like EC2 instances, security groups and load balancers, etc.  As such, you don't get charged for using EB, rather you get charged for the resources it stands up.  Basically, it's probably what I could have really used earlier/above, when trying to use an [EC2 instance with a load balancer](#using-a-load-balancer).  I imagine that it automatically sets up one of the things I was missing, or had misconfigured, when trying to do all this earlier.

As of the time of writing, EB only supports using a handful of languages/systems for deploying applications directly (though the ones they support probably do cover the vast majority of web applications out there), but fortunately one of the supported ones is Node.js.  EB also supports deploying Docker containers.  Thus, I shall attempt to do a direct Node.js deployment, and a Dockerised deployment.

### Node.js Direct

I'm not totally sure on what is required to deploy a Node.js application directly onto EB.  Probably, in part at least, due to my relative unfamiliarity with Node.js development overall.  Fortunately, there is some discussion on that point in the docs:

> You can include a Package.json file in your source bundle to install packages during deployment, to provide a start command, and to specify the Node.js version that you want your application to use. You can include an npm-shrinkwrap.json file to lock down dependency versions.
> ...
> There are several options to start your application. You can add a Procfile to your source bundle to specify the command that starts your application. If you don't provide a Procfile but provide a package.json file, Elastic Beanstalk runs npm start. If you don't provide that either, Elastic Beanstalk looks for the app.js or server.js file, in this order, and runs the script.

OJS doesn't contain a Procfile, but it _does_ come with a package.json file.  So, for a first attempt I will try simply uploading the v15 release .zip bundle and see if EB can take it from there.

The EB documentation [states](https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/create_deploy_nodejs.container.html) that it uses [NGINX](https://nginx.org/) as a proxy server (I presume they specifically mean a "_reverse_ proxy server") by default.  While this seems like a perfectly reasonable choice of proxy server, I can't find anywhere in the OJS any reference to use of a proxy server.  Searching the files for "nginx" returns no results, and the only mention of Apache that I can find is inside the Vagrantfile.  Digging into the Node.js:18 docker image's (which is also the 18-bookworm image) Dockerfile, I can't see anything that looks like inclusion of a proxy server, but I _can_ see that it is built atop buildpack-deps:bookworm image.  That image's Dockerfile, in turn, also does not contain any references to anything that looks like a proxy server, but is built on buildpack-deps:bookworm-scm.  Same result, but that is built on buildpack-deps:bookworm-curl.  Same thing again, but pointing back to debian:bookworm.  Of course, I could be wrong, but I doubt the base Debian image comes with something like Nginx or Apache installed by default.

The point of all of this is that it looks like OJS only uses the built-in HTTP server from Node.js, and makes no use of a proxy server when deploying via the container.  Perhaps the idea is that either you are working with it locally, in which case such a server would be complete overkill, or it is up to you to handle putting a proxy server in front of it yourself.  I'm not sure—OJS' deployment instructions make zero mention of the matter.  Again, this is probably only perceived by me as an issue because I'm not really familiar with how things are done in Node.js world.  Looking at the Node.js documentation [on setting up a Docker container for development](https://nodejs.org/en/docs/guides/nodejs-docker-webapp), they describe using [Express.js](https://expressjs.com/) to create a basic server.  OJS appears (roughly) to mirror this approach.  The [Express docs](https://expressjs.com/en/starter/hello-world.html) appear to suggest that their server uses [RunKit](https://runkit.com/home), but I can't figure anything out beyond that.  So, in summary, I'm uncertain what is actually powering OJS to handle web requests, but there doesn't seem to be anything to do with a proxy server.  Hopefully using EB doesn't muck something up.

Right, with that whole issue non-concluded, onto attempting a first deployment!  As I mentioned earlier, for a first attempt I will just plug in the .zip of the [v15 release](https://github.com/juice-shop/juice-shop/releases/tag/v15.0.0) from GitHub, and see what happens...

{{< figure src="OJS_EB_initial_setup.png" title="Initial Application and Environment Setup" alt="A screenshot from the AWS Elastic Beanstalk console showing the environment configuration interface." >}}
{{< figure src="OJS_EB_platform_select.png" title="Selecting the Platform and Application Code" alt="A screenshot from the AWS Elastic Beanstalk console showing the platform selection and application code configuration options." >}}
{{< figure src="OJS_EB_configuring_service_access_1.png" title="Configuring the appropriate Service Access, Take One" alt="A screenshot from the AWS Elastic Beanstalk console showing the selection of a service role, EC2 key pair, and EC2 instance profile." >}}
{{< figure src="OJS_EB_networking_config_noop.png" title="Configuring the network, by doing nothing at all" alt="A screenshot from the AWS Elastic Beanstalk console showing the network setup interface, but with everything set to the default settings." >}}
{{< figure src="OJS_EB_configure_traffic_scaling_noop.png" title="Configuring Traffic and Scaling by doing nothing at all" alt="A screenshot from the AWS Elastic Beanstalk console showing the instance traffic and scaling options, with everything set to the defaults." >}}
{{< figure src="OJS_EB_configure_monitoring_logging_noop.png" title="Configuring Updates, Logging and Monitoring by doing nothing at all" alt="A screenshot from the AWS Elastic Beanstalk console showing the settings for updates, monitoring and logging of an Elastic Beanstalk application, but with everything set to the defaults." >}}

#### So many difficulties...

I hit a bit of a stumbling block when I got to the point of trying to figure out what VPC I should use.  I don't _want_ to use the default one, but I'm also uncertain of what a custom VPC needs for EB's purposes.  I noticed, however, that this whole bit is described as optional, so _hopefully_ EB will use some sort of sensible approach on its own.  I choose not to configure anything, and just click `Next` and move on.  This is also the page where you configure connecting your application to an RDS database, but since OJS doesn't seem to use any external DB I choose not to enable that.

I'm even more iffy about the following page.  I do note with the 'Capacity' section, it says that you can use a single instance and only switch to using a load balancer when needed later on.  I'm not sure if that will mean that the whole thing will only be available on HTTP again, rather than HTTPS, given that previously it was supposed to be the load balancer that would handle TLS.  Alternatively, perhaps that duty is now left up to the provided proxy server.  Anyway, again I don't change anything and just carry on.

Same story again with the next page.  I don't know for sure that I should change anything, so I'll just leave it all alone for now.

Finally, I get to the review page, and hit `Submit` to try to deploy everything, and...  get hit with a wholly unclear error message.  I _think_ it's referring to my not having chosen an EC2 instance profile on the 'Configure service access' page.  I had thought that EB was supposed to set up a sensible default for you if you didn't fill it in, but perhaps I'm confusing it with the service role.  Digging back into the [EB documentation](https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/iam-instanceprofile.html), I find this:

> Previously Elastic Beanstalk created a default EC2 instance profile named aws-elasticbeanstalk-ec2-role the first time an AWS account created an environment. This instance profile included default managed policies. If your account already has this instance profile, it will remain available for you to assign to your environments.
>
>However, recent AWS security guidelines don’t allow an AWS service to automatically create roles with trust policies to other AWS services, EC2 in this case. Because of these security guidelines, Elastic Beanstalk no longer creates a default aws-elasticbeanstalk-ec2-role instance profile.

 Most likely, I was misremembering that bit.  Eventually I'm able to figure out the relevant instructions to create the required instance profile and try again.  Except, that still doesn't do it.  Same utterly unhelpful error message as before.  At this point, I'm really rather at a loss.  The only other thing I can think of that might need twiddling is the VPC.  Thus, I go just choose the default VPC and select the default subnet for every availability zone.  Same result.

 {{< figure src="OJS_EB_unhelpful_error_message.png" title="The spectacularly unhelpful error message" alt="A screenshot of an error message from AWS, which provides no useful information whatsoever on what the error was." >}}

##### Resorting to the Example Application
 
Possibly it's something to do with the OJS zip upload.  To double-check that, I will try to do the whole process again, except this time I will just use [the zip provided](https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/samples/nodejs.zip) for a basic 'getting started' sample application.  That _does_ work, with me making all the same choices for the application environment.  Thus, presumably the problem relates to the OJS zip file that I have been attempting to upload.  Unfortunately, I don't know _what_ the problem is.  Maybe instead of the base .zip, I need to use one of the platform-specific built releases available from the OJS GitHub instead (again, this is probably all an issue because I'm not really familiar with how Node.js does things).  After having confirmed that this does, in fact, deploy, I of course tidy the whole thing up again.  Sadly, it looks like this single-unit deployment did _not_ use HTTPS and rather just HTTP.  Which is a disappointment, since that was one of the key things I was hoping EB would do for me.

{{< figure src="OJS_EB_sample_application_landing.png" title="The landing page for the sample Node.js application" alt="A screenshot of the landing page for the Elastic Beanstalk Node.js sample application, demonstrating that this application was deployed by Elastic Beanstalk and accessible online." >}}

##### Back to the OJS

