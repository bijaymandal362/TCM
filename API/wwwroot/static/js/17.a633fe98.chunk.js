(this.webpackJsonptcm=this.webpackJsonptcm||[]).push([[17],{444:function(e,t,r){"use strict";r.d(t,"b",(function(){return i})),r.d(t,"c",(function(){return l})),r.d(t,"d",(function(){return o})),r.d(t,"a",(function(){return j}));var n=r(34),a=r.n(n),c=r(59),s=r(55),u=r(43),i=function(){var e=Object(c.a)(a.a.mark((function e(t){var r;return a.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:return Object(u.f)(),e.prev=1,e.next=4,Object(s.a)({url:"/User/GetUserListFilterByRole",method:"GET",params:t});case 4:return r=e.sent,e.abrupt("return",[r,null]);case 8:return e.prev=8,e.t0=e.catch(1),e.abrupt("return",[null,e.t0]);case 11:case"end":return e.stop()}}),e,null,[[1,8]])})));return function(t){return e.apply(this,arguments)}}(),l=function(){var e=Object(c.a)(a.a.mark((function e(){var t;return a.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:return Object(u.f)(),e.prev=1,e.next=4,Object(s.a)({url:"/User/GetRoleList",method:"GET"});case 4:return t=e.sent,e.abrupt("return",[t,null]);case 8:return e.prev=8,e.t0=e.catch(1),e.abrupt("return",[null,e.t0]);case 11:case"end":return e.stop()}}),e,null,[[1,8]])})));return function(){return e.apply(this,arguments)}}(),o=function(){var e=Object(c.a)(a.a.mark((function e(t){var r;return a.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:return Object(u.f)(),e.prev=1,e.next=4,Object(s.a)({url:"/User/UpdateUser",method:"POST",params:t});case 4:return r=e.sent,e.abrupt("return",[r,null]);case 8:return e.prev=8,e.t0=e.catch(1),e.abrupt("return",[null,e.t0]);case 11:case"end":return e.stop()}}),e,null,[[1,8]])})));return function(t){return e.apply(this,arguments)}}(),j=function(){var e=Object(c.a)(a.a.mark((function e(t){var r,n;return a.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:return Object(u.f)(),e.prev=1,r="/User/GetUserDetail/".concat(t),e.next=5,Object(s.a)({url:r,method:"GET"});case 5:return n=e.sent,e.abrupt("return",[n,null]);case 9:return e.prev=9,e.t0=e.catch(1),e.abrupt("return",[null,e.t0]);case 12:case"end":return e.stop()}}),e,null,[[1,9]])})));return function(t){return e.apply(this,arguments)}}()},485:function(e,t,r){"use strict";r.r(t);var n=r(34),a=r.n(n),c=r(59),s=r(50),u=r(0),i=r(450),l=r(477),o=r(216),j=r(475),b=r(459),d=r(442),p=r(443),O=r(400),f=r(476),m=r(406),x=r(457),h=r(471),v=r(478),y=r(16),g=r(444),w=r(219),k=r(416),S=r(198),N=r(39),U=r(433),I=r.n(U),C=r(4);t.default=function(){Object(y.g)();var e=Object(u.useState)([]),t=Object(s.a)(e,2),r=t[0],n=t[1],U=Object(u.useState)([]),G=Object(s.a)(U,2),E=G[0],z=G[1],A=Object(u.useState)(!1),P=Object(s.a)(A,2),B=P[0],J=P[1],L=Object(u.useState)(1),T=Object(s.a)(L,2),D=T[0],K=T[1],M=Object(u.useState)("name"),R=Object(s.a)(M,2),Y=R[0],q=R[1],F=Object(u.useState)(null),V=Object(s.a)(F,2),_=V[0],H=V[1],Q=Object(u.useState)(""),W=Object(s.a)(Q,2),X=W[0],Z=W[1],$=Object(v.a)(X,1e3),ee=Object(s.a)($,1)[0],te=[{title:"Name",key:"name",dataIndex:"name",render:function(e,t){var r=(null===t||void 0===t?void 0:t.email)||"email not set";return Object(C.jsxs)("div",{className:"d-flex justify-content-left align-items-center",children:[Object(C.jsx)(i.a,{src:"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSLIy6J349mbu7PUEYc5MLJ-G22K1R_lkK2ow&usqp=CAU"}),Object(C.jsxs)("div",{children:[Object(C.jsx)("p",{children:e}),Object(C.jsx)("p",{children:r})]})]})}},{title:"Projects",key:"projectCount",dataIndex:"projectCount"},{title:"Created On",key:"createdOn",dataIndex:"createdOn",render:function(e){return Object(C.jsx)("span",{children:e?I()(e).local().format("YY/MM/DD hh:mm"):""})}},{title:"Action",key:"name",render:function(e,t){return Object(C.jsxs)(l.b,{size:"middle",children:[Object(C.jsx)(N.b,{to:"/admin/users/".concat(t.userId),children:Object(C.jsx)(w.a,{})}),Object(C.jsx)(N.b,{to:"",children:Object(C.jsx)(k.a,{})})]})}}],re=function(){var e=Object(c.a)(a.a.mark((function e(){var t,r,c,u,i;return a.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:if(_){e.next=2;break}return e.abrupt("return");case 2:return J(!0),n([]),t={PageNumber:D,SearchValue:ee,PageSize:25,roleId:_},e.next=7,Object(g.b)(t);case 7:r=e.sent,c=Object(s.a)(r,2),u=c[0],i=c[1],J(!1),u&&n(u.data.data),i&&o.b.error("error");case 14:case"end":return e.stop()}}),e)})));return function(){return e.apply(this,arguments)}}(),ne=function(){var e=Object(c.a)(a.a.mark((function e(){var t,r,n,c,u;return a.a.wrap((function(e){for(;;)switch(e.prev=e.next){case 0:return e.next=2,Object(g.c)();case 2:t=e.sent,r=Object(s.a)(t,2),n=r[0],c=r[1],n&&(u=n.data,Array.isArray(u)&&u.length>0&&H(String(u[0].roleId)),z(u)),c&&o.b.error("Error loading user role metadata");case 8:case"end":return e.stop()}}),e)})));return function(){return e.apply(this,arguments)}}();return Object(u.useEffect)((function(){ne(),re()}),[]),Object(u.useEffect)((function(){(D||ee)&&re()}),[D,ee,_]),Object(C.jsxs)("div",{className:"user-list-page mt-4  mx-auto",children:[Object(C.jsx)(j.a,{title:"Users",extra:[]}),Object(C.jsx)("div",{children:Object(C.jsx)(b.a,{})}),Object(C.jsx)(d.a,{className:"d-flex justify-content-between",children:Object(C.jsx)(p.a,{span:24,children:Object(C.jsx)(O.a,{mode:"horizontal",onClick:function(e){H(e.key)},style:{background:"none",borderBottom:"none"},overflowedIndicator:"",selectedKeys:_?[_]:[],children:E.map((function(e){return Object(C.jsxs)(O.a.Item,{children:[e.name," ",Object(C.jsx)(f.a,{children:e.userCount})]},e.roleId)}))})})}),Object(C.jsxs)(d.a,{className:"d-flex justify-content-between",children:[Object(C.jsx)(p.a,{lg:16,md:16,xs:24,className:"pe-0 pe-md-2",children:Object(C.jsx)(m.a,{size:"large",prefix:Object(C.jsx)(S.a,{style:{color:"inherit"}}),placeholder:"Filter By Name...",value:X,onChange:function(e){Z(e.target.value)}})}),Object(C.jsxs)(p.a,{span:8,xs:24,md:8,className:"d-flex align-items-center justify-content-center mt-2 mt-md-auto  ",children:[Object(C.jsx)("span",{className:"me-2 text-no-wrap",children:"Sort By:"}),Object(C.jsx)(x.a,{size:"large",value:Y,options:[{label:"Name",value:"name"}],onSelect:function(e){return q(e)},className:"w-100"})]})]}),Object(C.jsx)(b.a,{}),Object(C.jsx)(d.a,{children:Object(C.jsx)(p.a,{span:"24",children:Object(C.jsx)(h.a,{className:"table table-responsive",dataSource:r,columns:te,loading:B,pagination:{onChange:function(e){K(e)},pageSize:25}})})})]})}}}]);
//# sourceMappingURL=17.a633fe98.chunk.js.map